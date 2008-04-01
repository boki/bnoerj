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
#include "NativeEngine.h"

using namespace Bnoerj::Audio;
/*
AudioCategory::AudioCategory()
	: engine(nullptr)
	, name(nullptr)
	, category(0)
{
}
*/
AudioCategory::AudioCategory(AudioEngine^ engine, String^ name)
	: engine(engine)
	, name(name)
	, category(0)
{
	if (engine == nullptr)
	{
		throw gcnew ArgumentNullException("engine", StringResources::NullNotAllowed);
	}
	if (String::IsNullOrEmpty(name) == true)
	{
		throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
	}

	category = engine->engine->GetCategory(name);
}

String^ AudioCategory::Name::get()
{
	return name;
}

void AudioCategory::Pause()
{
	engine->engine->Pause(category, TRUE);
}

void AudioCategory::Resume()
{
	engine->engine->Pause(category, FALSE);
}

void AudioCategory::Stop(AudioStopOptions options)
{
	engine->engine->Stop(category, (DWORD)options);
}

void AudioCategory::SetVolume(float volume)
{
	engine->engine->SetVolume(category, volume);
}

bool AudioCategory::Equals(AudioCategory^ other)
{
	return engine == other->engine && category == other->category;
}

bool AudioCategory::Equals(Object^ other)
{
	if (dynamic_cast<AudioCategory^>(other) != nullptr)
	{
		return Equals(safe_cast<AudioCategory^>(other));
	}
	return false;
}

int AudioCategory::GetHashCode()
{
	int hash = engine != nullptr ? engine->GetHashCode() : 0;
	return hash ^ category.GetHashCode();
}

bool AudioCategory::operator ==(AudioCategory^ a, AudioCategory^ b)
{
	return a->engine == b->engine && a->category == b->category;
}

bool AudioCategory::operator !=(AudioCategory^ a, AudioCategory^ b)
{
	return a->engine != b->engine || a->category != b->category;
}
