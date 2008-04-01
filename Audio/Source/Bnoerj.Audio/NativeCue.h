// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

#include "NativeAudioObject.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace Bnoerj { namespace Audio { namespace Native {

	ref class Cue : public Bnoerj::Audio::Native::AudioObject
	{
	public:
		Cue(IXACT3Engine* pEngine, void* pObject)
			: AudioObject(pEngine, pObject)
		{}

		virtual void Release() override;

		DWORD GetStatus();

		void Pause(BOOL pause);
		void Play();
		void Stop(DWORD options);

		float GetVariable(String^ name);
		void SetVariable(String^ name, float value);
	};

}}}
