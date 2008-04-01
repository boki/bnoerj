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
#include "AudioListener.h"
#include "AudioEmitter.h"
#include "AudioObject.h"
#include "Cue.h"
#include "SoundBank.h"

#include "NativeEngine.h"
#include "NativeCue.h"
#include "NativeSoundBank.h"

using namespace System::IO;
using namespace Bnoerj::Audio;


SoundBank::SoundBank(AudioEngine^ engine, String^ filename)
{
	if (engine == nullptr)
	{
		throw gcnew ArgumentNullException("engine", StringResources::NullNotAllowed);
	}
	if (String::IsNullOrEmpty(filename) == true)
	{
		throw gcnew ArgumentNullException("filename", StringResources::NullNotAllowed);
	}

	this->nativeObject = gcnew Native::SoundBank(engine->pEngine, filename);
	engine->AddAudioInstance(this->nativeObject->pObject, this);

	this->engine = engine;
}

bool SoundBank::IsInUse::get()
{
	DWORD status = static_cast<Native::SoundBank^>(nativeObject)->GetStatus();
	return (status & XACT_SOUNDBANKSTATE_INUSE) != 0;
}

Cue^ SoundBank::GetCue(String^ name)
{
	if (String::IsNullOrEmpty(name) == true)
	{
		throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
	}
	Native::Cue^ nativeCue = static_cast<Native::SoundBank^>(nativeObject)->GetCue(name);
	return gcnew Cue(engine, static_cast<Native::AudioObject^>(nativeCue), name);
}

void SoundBank::PlayCue(String^ name)
{
	if (String::IsNullOrEmpty(name) == true)
	{
		throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
	}

	static_cast<Native::SoundBank^>(nativeObject)->PlayCue(name);
}

void SoundBank::PlayCue(String^ name, AudioListener^ listener, AudioEmitter^ emitter)
{
	if (String::IsNullOrEmpty(name) == true)
    {
        throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
    }

	Native::Cue^ nativeCue = static_cast<Native::SoundBank^>(nativeObject)->GetCue(name);
    Cue^ cue = gcnew Cue(engine, static_cast<Native::AudioObject^>(nativeCue), name);
    cue->Apply3D(listener, emitter);
    cue->Play();
}
