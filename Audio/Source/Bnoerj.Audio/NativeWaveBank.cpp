// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"

#include "NativeEngine.h"
#include "NativeWaveBank.h"
#include "NativeHelpers.h"
#include "ErrorToException.h"

using namespace System::IO;
using namespace Bnoerj::Audio::Native;
using namespace Bnoerj::Native::Helpers;

WaveBank::WaveBank(IXACT3Engine* pEngine, String^ filename)
{
	msclr::lock lock(Engine::syncRoot);

	array<Byte>^ aData = File::ReadAllBytes(filename);
	void* pData = NULL;
	if (aData != nullptr && aData->Length > 0)
	{
		pin_ptr<Byte> ptrData = &aData[0];
		pData = new BYTE[aData->Length];
		memcpy_s(pData, aData->Length, ptrData, aData->Length);
	}

	IXACT3WaveBank* pWaveBank;
	HRESULT hr = pEngine->CreateInMemoryWaveBank(pData, aData->Length, 0, 0, &pWaveBank);
	if (FAILED(hr))
	{
		delete[] pData;
		ErrorToException::Throw(hr);
	}

	this->pEngine = pEngine;
	this->pObject = pWaveBank;
	this->pData = pData;
	hStreamingWaveBankFile = INVALID_HANDLE_VALUE;
}

WaveBank::WaveBank(IXACT3Engine* pEngine, String^ filename, DWORD offset, short packetSize)
{
	msclr::lock lock(Engine::syncRoot);

	hStreamingWaveBankFile = ::CreateFileW(
		Bnoerj::Native::Helpers::StringConverter::ToNativeStringUni(filename),
		GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING,
		FILE_FLAG_OVERLAPPED | FILE_FLAG_NO_BUFFERING, NULL);
	if (hStreamingWaveBankFile == INVALID_HANDLE_VALUE)
	{
		ErrorToException::Throw(E_FAIL);
	}

	XACT_WAVEBANK_STREAMING_PARAMETERS params = { 0 };
	params.file = hStreamingWaveBankFile;
	params.offset = offset;
	params.packetSize = packetSize;

	IXACT3WaveBank* pWaveBank;
	HRESULT hr = pEngine->CreateStreamingWaveBank(&params, &pWaveBank);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}

	this->pEngine = pEngine;
	this->pObject = pWaveBank;
	this->pData = NULL;
}

void WaveBank::Release()
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3WaveBank* pWaveBank = static_cast<IXACT3WaveBank*>(pObject);
	if (pWaveBank != NULL)
	{
		pWaveBank->Destroy();
	}
	pObject = NULL;

	if (this->pData != NULL)
	{
		BYTE* pData = static_cast<BYTE*>(this->pData);
		delete[] pData;
	}
	pData = NULL;

	if (hStreamingWaveBankFile != INVALID_HANDLE_VALUE)
	{
		::CloseHandle(hStreamingWaveBankFile);
		hStreamingWaveBankFile = INVALID_HANDLE_VALUE;
	}
}

DWORD WaveBank::GetStatus()
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3WaveBank* pWaveBank = static_cast<IXACT3WaveBank*>(pObject);
	DWORD state;
	HRESULT hr = pWaveBank->GetState(&state);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
	return state;
}
