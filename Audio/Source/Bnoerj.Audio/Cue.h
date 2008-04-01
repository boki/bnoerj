// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

#include "NativeAudioObject.h"

using namespace System;

namespace Bnoerj { namespace Audio {

	public ref class Cue : public AudioObject
	{
		String^ name;
		bool played;
		bool applied3D;

	internal:
		Cue(AudioEngine^ engine, Native::AudioObject^ nativeObject, String^ name);

	public:
		property bool IsCreated { bool get(); }
		property bool IsPrepared { bool get(); }
		property bool IsPreparing { bool get(); }

		property bool IsPlaying { bool get(); }
		property bool IsPaused { bool get(); }
		property bool IsStopped { bool get(); }
		property bool IsStopping { bool get(); }

		property String^ Name { String^ get(); }

		void Apply3D(AudioListener^ listener, AudioEmitter^ emitter);

		float GetVariable(String^ name);
		void SetVariable(String^ name, float value);

		void Play();
		void Pause();
		void Resume();
		void Stop(AudioStopOptions options);
	};
}}
