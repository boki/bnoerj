// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"

#include "StringResources.h"

#include "AudioStopOptions.h"
#include "AudioCategory.h"
#include "RendererDetail.h"
#include "AudioEngine.h"
#include "AudioObject.h"
#include "AudioListener.h"
#include "AudioEmitter.h"
#include "Cue.h"

#include "NativeEngine.h"
#include "NativeCue.h"

using namespace Bnoerj::Audio;

Cue::Cue(AudioEngine^ engine, Native::AudioObject^ nativeObject, String^ name)
	: AudioObject(engine, nativeObject)
	, name(name)
{
}

bool Cue::IsCreated::get()
{
	DWORD status = static_cast<Native::Cue^>(nativeObject)->GetStatus();
	return (status & XACT_CUESTATE_CREATED) != 0;
}

bool Cue::IsPrepared::get()
{
	DWORD status = static_cast<Native::Cue^>(nativeObject)->GetStatus();
	return (status & XACT_CUESTATE_PREPARED) != 0;
}

bool Cue::IsPreparing::get()
{
	DWORD status = static_cast<Native::Cue^>(nativeObject)->GetStatus();
	return (status & XACT_CUESTATE_PREPARING) != 0;
}

bool Cue::IsPlaying::get()
{
	DWORD status = static_cast<Native::Cue^>(nativeObject)->GetStatus();
	return (status & XACT_CUESTATE_PLAYING) != 0;
}

bool Cue::IsPaused::get()
{
	DWORD status = static_cast<Native::Cue^>(nativeObject)->GetStatus();
	return (status & XACT_CUESTATE_PAUSED) != 0;
}

bool Cue::IsStopped::get()
{
	DWORD status = static_cast<Native::Cue^>(nativeObject)->GetStatus();
	return (status & XACT_CUESTATE_STOPPED) != 0;
}

bool Cue::IsStopping::get()
{
	DWORD status = static_cast<Native::Cue^>(nativeObject)->GetStatus();
	return (status & XACT_CUESTATE_STOPPING) != 0;
}

String^ Cue::Name::get()
{
	return name;
}

void Cue::Apply3D(AudioListener^ listener, AudioEmitter^ emitter)
{
	if (listener == nullptr)
	{
		throw gcnew ArgumentNullException("listener", StringResources::NullNotAllowed);
	}
	if (emitter == nullptr)
	{
		throw gcnew ArgumentNullException("emitter", StringResources::NullNotAllowed);
	}

	if (applied3D == false && played == true)
	{
		throw gcnew InvalidOperationException(StringResources::Apply3DBeforePlaying);
	}

	void* pCue = static_cast<Native::Cue^>(nativeObject)->pObject;
	engine->engine->Apply3D(static_cast<IXACT3Cue*>(pCue), listener->listenerData, emitter->emitterData);

	applied3D = true;
}

float Cue::GetVariable(String^ name)
{
	if (String::IsNullOrEmpty(name) == true)
	{
		throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
	}
	return static_cast<Native::Cue^>(nativeObject)->GetVariable(name);
}

void Cue::SetVariable(String^ name, float value)
{
	if (String::IsNullOrEmpty(name) == true)
	{
		throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
	}
	static_cast<Native::Cue^>(nativeObject)->SetVariable(name, value);
}

void Cue::Play()
{
	static_cast<Native::Cue^>(nativeObject)->Play();
	played = true;
}

void Cue::Pause()
{
	static_cast<Native::Cue^>(nativeObject)->Pause(TRUE);
}

void Cue::Resume()
{
	static_cast<Native::Cue^>(nativeObject)->Pause(FALSE);
}

void Cue::Stop(AudioStopOptions options)
{
	static_cast<Native::Cue^>(nativeObject)->Stop(static_cast<DWORD>(options));
}
