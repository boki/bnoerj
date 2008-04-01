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

	ref class Cue;

	ref class SoundBank : public AudioObject
	{
	public:
		SoundBank(IXACT3Engine* pEngine, String^ filename);

		virtual void Release() override;

		Native::Cue^ GetCue(String^ name);
		DWORD GetStatus();
		void PlayCue(String^ name);
	};

}}}
