// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"

#include "NativeEngine.h"
#include "NativeHelpers.h"
#include "ErrorToException.h"

using namespace System::IO;
using namespace Bnoerj::Audio::Native;
using namespace Bnoerj::Native::Helpers;

//-----------------------------------------------------------------------------------------
// This is the callback for handling XACT notifications.  This callback can be executed on a
// different thread than the app thread so shared data must be thread safe.  The game
// also needs to minimize the amount of time spent in this callbacks to avoid glitching,
// and a limited subset of XACT API can be called from inside the callback so
// it is sometimes necessary to handle the notification outside of this callback.
//-----------------------------------------------------------------------------------------
void WINAPI XACTNotificationCallback(const XACT_NOTIFICATION* pNotification)
{
	// Use the critical section properly to make shared data thread safe while avoiding deadlocks.  
	//
	// To do this follow this advice:
	// 1) Use a specific CS only to protect the specific shared data structures between the callback and the app thread.
	// 2) Don’t make any API calls while holding the CS. Use it to access the shared data, make a local copy of the data, release the CS and then make the API call.
	// 3) Spend minimal amount of time in the CS (to prevent the callback thread from waiting too long causing a glitch).   
	// 
	// Instead of using a CS, you can also use a non-blocking queues to keep track of notifications meaning 
	// callback will push never pop only push and the app thread will only pop never push

	if (pNotification->type == XACTNOTIFICATIONTYPE_CUEDESTROYED)
	{
		msclr::lock lock(Engine::syncRoot);

		Engine::CueDestroyed(IntPtr(pNotification->cue.pCue));
	}
}

Engine::Engine(String^ settingsFilename, unsigned int lookAheadTime, Guid rendererId)
	: AudioObject()
	, p3DAudioData(NULL)
	, pDsp(NULL)
	, pDelayTimes(NULL)
	, pMatrixCoefficients(NULL)
{
    // Enable run-time memory check for debug builds.
#if defined(DEBUG) | defined(_DEBUG) | defined(CHECKED_BUILD)
	::_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	//
	// Create the XACT3 engine
	//

	HRESULT hr;
	{
		DWORD dwCreationFlags = 0;
#if defined(DEBUG) | defined(_DEBUG) | defined(CHECKED_BUILD)
	    dwCreationFlags |= XACT_FLAG_API_DEBUG_MODE;
#endif
		IXACT3Engine* pEngine = NULL;
		HRESULT hr = ::XACT3CreateEngine(dwCreationFlags, &pEngine);
		if (FAILED(hr) || pEngine == NULL)
		{
			ErrorToException::Throw(hr);
		}
		this->pEngine = pEngine;
	}

	//
	// Get sttings data
	//

	array<Byte>^ aData = File::ReadAllBytes(settingsFilename);
	if (aData != nullptr && aData->Length > 0)
	{
		pin_ptr<Byte> pSettings = &aData[0];
		pData = new BYTE[aData->Length];
		memcpy_s(pData, aData->Length, pSettings, aData->Length);
	}

	//
	// Initialize XACT
	//

	XACT_RUNTIME_PARAMETERS xactRtParams = { 0 };
	xactRtParams.lookAheadTime = lookAheadTime;
	if (pData != NULL && aData->Length > 0)
	{
		xactRtParams.pGlobalSettingsBuffer = pData;
		xactRtParams.globalSettingsBufferSize = aData->Length;
	}
	xactRtParams.fnNotificationCallback = XACTNotificationCallback;
	if (rendererId != Guid::Empty)
	{
		xactRtParams.pRendererID = StringConverter::ToNativeStringUni(rendererId.ToString("B"));
	}
    hr = pEngine->Initialize(&xactRtParams);
    if (FAILED(hr))
	{
		pEngine->Release();
		delete[] pData;
        ErrorToException::Throw(hr);
	}

	// Get the number of channels on the final mix
	WAVEFORMATEXTENSIBLE wfxFinalMixFormat;
	hr = pEngine->GetFinalMixFormat(&wfxFinalMixFormat);
	if (FAILED(hr))
	{
		pEngine->Release();
		delete[] pData;
		ErrorToException::Throw(hr);
	}
	destinationChannelCount = wfxFinalMixFormat.Format.nChannels;

	//
	// Register for XACT notifications
	//

	XACT_NOTIFICATION_DESCRIPTION desc = {0};
	// Use "cue destroyed" notification to cleanup the managed cues
	desc.flags = XACT_FLAG_NOTIFICATION_PERSIST;
	desc.type = XACTNOTIFICATIONTYPE_CUEDESTROYED;
	desc.cueIndex = XACTINDEX_INVALID;
	pEngine->RegisterNotification(&desc);

	//
	// Initialize 3D audio
	//

	X3DAUDIO_HANDLE h3DAudio;
	hr = ::XACT3DInitialize(pEngine, h3DAudio);
	if (FAILED(hr))
	{
		pEngine->Release();
		delete[] pData;
		ErrorToException::Throw(hr);
	}

	p3DAudioData = new BYTE[X3DAUDIO_HANDLE_BYTESIZE];
	memcpy_s(p3DAudioData, X3DAUDIO_HANDLE_BYTESIZE, h3DAudio, X3DAUDIO_HANDLE_BYTESIZE);

	pDelayTimes = new FLOAT32[2];
	::memset(pDelayTimes, 0, 2 * sizeof(FLOAT32));

	// Alloc coefficients for 7.1
	pMatrixCoefficients = new FLOAT32[2 * 8];
	::memset(pMatrixCoefficients, 0, 2 * 8 * sizeof(FLOAT32));
}

void Engine::Release()
{
	msclr::lock lock(Engine::syncRoot);

	pEngine->ShutDown();
	pEngine->Release();

	delete[] pData;
	pData = NULL;

	delete p3DAudioData;
	p3DAudioData = NULL;

	delete pDsp;
	pDsp = NULL;

	delete[] pDelayTimes;
	pDelayTimes = NULL;

	delete[] pMatrixCoefficients;
	pMatrixCoefficients = NULL;
}

int Engine::GetRendererCount()
{
	msclr::lock lock(Engine::syncRoot);

	XACTINDEX count = 0;
	pEngine->GetRendererCount(&count);

	return count;
}

void Engine::GetRendererDetail(int index, String^% friendlyName, String^% guid)
{
	msclr::lock lock(Engine::syncRoot);

	XACT_RENDERER_DETAILS rendererDetails = { 0 };
	pEngine->GetRendererDetails((XACTINDEX)index, &rendererDetails);
	friendlyName = StringConverter::ToString(rendererDetails.displayName);
	guid = StringConverter::ToString(rendererDetails.rendererID);
}

float Engine::GetGlobalVariable(String^ name)
{
	msclr::lock lock(Engine::syncRoot);

	PCSTR pName = StringConverter::ToNativeString(name);
	XACTVARIABLEINDEX index = pEngine->GetGlobalVariableIndex(pName);
	if (index == XACTVARIABLEINDEX_INVALID)
	{
		//ErrorToException::Throw(hr);
		return 0.0f;
	}

	XACTVARIABLEVALUE varValue;
	HRESULT hr = pEngine->GetGlobalVariable(index, &varValue);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}

	return varValue;
}

void Engine::SetGlobalVariable(String^ name, float value)
{
	msclr::lock lock(Engine::syncRoot);

	PCSTR pName = StringConverter::ToNativeString(name);
	XACTVARIABLEINDEX index = pEngine->GetGlobalVariableIndex(pName);
	if (index == XACTVARIABLEINDEX_INVALID)
	{
		//ErrorToException::Throw(hr);
		return;
	}

	XACTVARIABLEVALUE varValue = value;
	HRESULT hr = pEngine->SetGlobalVariable(index, varValue);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}

XACTCATEGORY Engine::GetCategory(String^ name)
{
	msclr::lock lock(Engine::syncRoot);

	PCSTR pName = StringConverter::ToNativeString(name);
	XACTCATEGORY category = pEngine->GetCategory(pName);
	if (category == XACTCATEGORY_INVALID)
	{
		throw gcnew InvalidOperationException(StringResources::CouldNotCreateResource);
	}

	return category;
}

void Engine::Update()
{
	msclr::lock lock(Engine::syncRoot);

	pEngine->DoWork();
}

void Engine::Pause(XACTCATEGORY cateorgy, BOOL pause)
{
	msclr::lock lock(Engine::syncRoot);

	HRESULT hr = pEngine->Pause(cateorgy, pause);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}

void Engine::Stop(XACTCATEGORY cateorgy, DWORD options)
{
	msclr::lock lock(Engine::syncRoot);

	HRESULT hr = pEngine->Stop(cateorgy, options);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}

void Engine::SetVolume(XACTCATEGORY cateorgy, float volume)
{
	msclr::lock lock(Engine::syncRoot);

	HRESULT hr = pEngine->SetVolume(cateorgy, volume);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}

void Engine::Apply3D(IXACT3Cue* pCue, X3DAUDIO_LISTENER* pListener, X3DAUDIO_EMITTER* pEmitter)
{
	msclr::lock lock(Engine::syncRoot);

	if (pDsp == NULL)
	{
		pDsp = new X3DAUDIO_DSP_SETTINGS();
		ZeroMemory(pDsp, sizeof(X3DAUDIO_DSP_SETTINGS));
	}

	if (pDsp->pDelayTimes != pDelayTimes || destinationChannelCount != pDsp->DstChannelCount)
	{
		pDsp->pMatrixCoefficients = pMatrixCoefficients;
		pDsp->pDelayTimes = pDelayTimes;
		pDsp->SrcChannelCount = 2;
		pDsp->DstChannelCount = destinationChannelCount;
	}

	HRESULT hr = ::XACT3DCalculate(p3DAudioData, pListener, pEmitter, pDsp);
	if (SUCCEEDED(hr))
	{
		::XACT3DApply(pDsp, pCue);
	}
}
