// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"

#include "NativeEngine.h"
#include "NativeCue.h"
#include "NativeHelpers.h"
#include "ErrorToException.h"

using namespace System::IO;
using namespace Bnoerj::Audio::Native;
using namespace Bnoerj::Native::Helpers;

void Cue::Release()
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3Cue* pCue = static_cast<IXACT3Cue*>(pObject);
	pCue->Destroy();
	pObject = NULL;
}

DWORD Cue::GetStatus()
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3Cue* pCue = static_cast<IXACT3Cue*>(pObject);
	DWORD state;
	HRESULT hr = pCue->GetState(&state);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
	return state;
}

void Cue::Pause(BOOL pause)
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3Cue* pCue = static_cast<IXACT3Cue*>(pObject);
	HRESULT hr = pCue->Pause(pause);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}

void Cue::Play()
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3Cue* pCue = static_cast<IXACT3Cue*>(pObject);
	HRESULT hr = pCue->Play();
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}

void Cue::Stop(DWORD options)
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3Cue* pCue = static_cast<IXACT3Cue*>(pObject);
	HRESULT hr = pCue->Stop(options);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}

float Cue::GetVariable(String^ name)
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3Cue* pCue = static_cast<IXACT3Cue*>(pObject);

	PCSTR pName = StringConverter::ToNativeString(name);
	XACTVARIABLEINDEX index = pCue->GetVariableIndex(pName);
	if (index == XACTVARIABLEINDEX_INVALID)
	{
		//ErrorToException::Throw(hr);
		return 0.0f;
	}

	float value;
	HRESULT hr = pCue->GetVariable(index, &value);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}

	return value;
}

void Cue::SetVariable(String^ name, float value)
{
	msclr::lock lock(Engine::syncRoot);

	IXACT3Cue* pCue = static_cast<IXACT3Cue*>(pObject);

	PCSTR pName = StringConverter::ToNativeString(name);
	XACTVARIABLEINDEX index = pCue->GetVariableIndex(pName);
	if (index == XACTVARIABLEINDEX_INVALID)
	{
		//ErrorToException::Throw(hr);
		return;
	}

	HRESULT hr = pCue->SetVariable(index, value);
	if (FAILED(hr))
	{
		ErrorToException::Throw(hr);
	}
}
