// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"

#include "NativeEngine.h"
#include "NativeCue.h"
#include "NativeSoundBank.h"
#include "NativeHelpers.h"
#include "ErrorToException.h"

using namespace System::IO;
using namespace Bnoerj::Audio::Native;
using namespace Bnoerj::Native::Helpers;

SoundBank::SoundBank(IXACT3Engine* pEngine, String^ filename)
{
	msclr::lock lock(Engine::syncRoot);

	array<Byte>^ aData = File::ReadAllBytes(filename);
	void* pData = NULL;
	if (aData != nullptr && aData->Length > 0)
	{
		pin_ptr<Byte> pSettings = &aData[0];
		pData = new BYTE[aData->Length];
		memcpy_s(pData, aData->Length, pSettings, aData->Length);
	}

	IXACT3SoundBank* pSoundBank;
	HRESULT hr = pEngine->CreateSoundBank(pData, aData->Length, 0, 0, &pSoundBank);
	if (FAILED(hr))
	{
		delete[] pData;
		ErrorToException::Throw(hr);
	}

	this->pEngine = pEngine;
	this->pData = pData;
	this->pObject = pSoundBank;
}

void SoundBank::Release()
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3SoundBank* pSoundBank = static_cast<IXACT3SoundBank*>(pObject);
	pSoundBank->Destroy();

	BYTE* pData = static_cast<BYTE*>(this->pData);
	delete[] pData;
}

Cue^ SoundBank::GetCue(String^ name)
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3SoundBank* pSoundBank = static_cast<IXACT3SoundBank*>(pObject);

	PCSTR pName = StringConverter::ToNativeString(name);
	XACTINDEX index = pSoundBank->GetCueIndex(pName);
	if (index == XACTINDEX_INVALID)
	{
		return nullptr;
	}

	IXACT3Cue* pCue;
	HRESULT hr = pSoundBank->Prepare(index, 0, 0, &pCue);
	if (FAILED(hr))
	{
		return nullptr;
	}
	return gcnew Cue(pEngine, pCue);
}

DWORD SoundBank::GetStatus()
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3SoundBank* pSoundBank = static_cast<IXACT3SoundBank*>(pObject);
	DWORD state;
	HRESULT hr = pSoundBank->GetState(&state);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
	return state;
}

void SoundBank::PlayCue(String^ name)
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3SoundBank* pSoundBank = static_cast<IXACT3SoundBank*>(pObject);

	PCSTR pName = StringConverter::ToNativeString(name);
	XACTINDEX index = pSoundBank->GetCueIndex(pName);
	if (index == XACTINDEX_INVALID)
	{
		return;
		//ErrorToException::Throw(hr);
	}

	HRESULT hr = pSoundBank->Play(index, 0, 0, NULL);
	if (FAILED(hr))
	{
		return;
		//ErrorToException::Throw(hr);
	}
}
