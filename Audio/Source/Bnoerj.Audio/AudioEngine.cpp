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
#include "WaveBank.h"
#include "SoundBank.h"

#include "NativeEngine.h"
#include "NativeAudioObject.h"

using namespace System::IO;
using namespace Bnoerj::Audio;


AudioEngine::AudioEngine(String^ settingsFile)
	: isDisposed(false)
{
	Initialize(settingsFile, TimeSpan(0, 0, 0, 0, XACT_ENGINE_LOOKAHEAD_DEFAULT), Guid::Empty);
}

AudioEngine::AudioEngine(String^ settingsFile, TimeSpan lookAheadTime, Guid rendererId)
	: isDisposed(false)
{
	Initialize(settingsFile, lookAheadTime, rendererId);
}

void AudioEngine::Initialize(String^ settingsFile, TimeSpan lookAheadTime, Guid rendererId)
{
	if (String::IsNullOrEmpty(settingsFile) == true)
	{
		throw gcnew ArgumentNullException("settingsFile", StringResources::NullNotAllowed);
	}

	// Create XACT3 engine
	String^ fullPath = Path::GetFullPath(settingsFile);
	engine = gcnew Native::Engine(fullPath, (UInt32)lookAheadTime.TotalMilliseconds, rendererId);
	if (engine == nullptr)
	{
		throw gcnew InvalidOperationException(StringResources::CouldNotCreateResource);
	}
	pEngine = engine->pEngine;

	if (hookedCueDestroy == false)
	{
		Native::Engine::CueDestroyed += gcnew Native::CueDestroyedEventHandler(&AudioEngine::NotifyCueDestroyed);
		hookedCueDestroy = true;
	}
}

AudioEngine::~AudioEngine()
{
	msclr::lock lock(AudioEngine::syncRoot);

	// Save dispose state to raise the disposed event properly
	bool wasDisposed = isDisposed;

	lock.release();

	this->!AudioEngine();

	if (wasDisposed == false)
	{
		Disposing(this, EventArgs::Empty);
	}

	GC::SuppressFinalize(this);
}

AudioEngine::!AudioEngine()
{
	msclr::lock lock(AudioEngine::syncRoot);

	if (isDisposed == false)
	{
		isDisposed = true;
		if (engine != nullptr)
		{
			NotifyEngineDestroyed(this);

			delete engine;
			engine = nullptr;
		}
	}
}

bool AudioEngine::IsDisposed::get()
{
	return isDisposed;
}

ReadOnlyCollection<RendererDetail^>^ AudioEngine::RendererDetails::get()
{
	int rendererCount = engine->GetRendererCount();
	if (rendererCount <= 0)
	{
		return nullptr;
	}

	List<RendererDetail^> renderers(rendererCount);
	for (int i = 0; i < rendererCount; i++)
	{
		String^ friendlyName = String::Empty;
		String^ guid = String::Empty;
		engine->GetRendererDetail(i, friendlyName, guid);
		renderers.Add(gcnew RendererDetail(friendlyName, Guid(guid)));
	}
	return gcnew ReadOnlyCollection<RendererDetail^>(%renderers);
}

//event Disposing;

AudioCategory^ AudioEngine::GetCategory(String^ name)
{
	return gcnew AudioCategory(this, name);
}

float AudioEngine::GetGlobalVariable(String^ name)
{
	if (String::IsNullOrEmpty(name) == true)
	{
		throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
	}
	return engine->GetGlobalVariable(name);
}

void AudioEngine::SetGlobalVariable(String^ name, float value)
{
	if (String::IsNullOrEmpty(name) == true)
	{
		throw gcnew ArgumentNullException("name", StringResources::NullNotAllowed);
	}
	engine->SetGlobalVariable(name, value);
}

void AudioEngine::Update()
{
	engine->Update();
}

void AudioEngine::AddAudioInstance(void* ptr, Object^ instance)
{
	audioInstances->Add(IntPtr(ptr), gcnew WeakReference(instance, false));
}

void AudioEngine::RemoveAudioInstance(void* ptr)
{
	audioInstances->Remove(IntPtr(ptr));
}

void AudioEngine::NotifyCueDestroyed(IntPtr pCue)
{
	msclr::lock lock(syncRoot);

	WeakReference^ ref;
	if (audioInstances->TryGetValue(pCue, ref) == true)
	{
		audioInstances->Remove(pCue);
		Object^ target = nullptr;
		try
		{
			target = ref->Target;
		}
		catch(InvalidOperationException^)
		{
		}

		if (target != nullptr)
		{
			Cue^ cue = safe_cast<Cue^>(target);
			delete cue;
		}
	}
}

void AudioEngine::NotifyEngineDestroyed(AudioEngine^ engine)
{
	msclr::lock lock(syncRoot);

	List<WeakReference^>^ instances = gcnew List<WeakReference^>(audioInstances->Values);
	for each (WeakReference^ ref in instances)
	{
		Object^ target = nullptr;
		try
		{
			target = ref->Target;
		}
		catch(InvalidOperationException^)
		{
			continue;
		}

		if (target != nullptr)
		{
			AudioObject^ audioObject = dynamic_cast<AudioObject^>(target);
			Native::AudioObject^ nativeAudioObject = audioObject->nativeObject;
			if (audioObject != nullptr && nativeAudioObject != nullptr &&
				nativeAudioObject->pEngine == engine->pEngine)
			{
				delete audioObject;
			}
#if 0
			Cue^ cue = dynamic_cast<Cue^>(target);
			if (cue != nullptr)
			{
				if (cue->pEngine == engine->pEngine)
				{
					audioInstances->Remove(cue->pObject);
					cue->pObject = IntPtr::Zero;
					delete cue;
				}
				continue;
			}

			WaveBank^ waveBank = dynamic_cast<WaveBank^>(target);
			if (waveBank != nullptr)
			{
				if (waveBank->pEngine == engine->pEngine)
				{
					audioInstances->Remove(waveBank->pObject);
					waveBank->pObject = IntPtr::Zero;
					delete waveBank;
				}
				continue;
			}

			SoundBank^ soundBank = dynamic_cast<SoundBank^>(target);
			if (soundBank != nullptr)
			{
				if (soundBank->pEngine == engine->pEngine)
				{
					audioInstances->Remove(soundBank->pObject);
					soundBank->pObject = IntPtr::Zero;
					delete soundBank;
				}
				continue;
			}
#endif
		}
	}
}
