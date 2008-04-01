// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;

namespace Bnoerj { namespace Audio {

	public ref class SoundBank : public AudioObject
	{
	public:
		SoundBank(AudioEngine^ engine, String^ filename);

		property bool IsInUse { bool get(); }

		Cue^ GetCue(String^ name);
		void PlayCue(String^ name);
		void PlayCue(String^ name, AudioListener^ listener, AudioEmitter^ emitter);
	};

}}
