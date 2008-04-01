// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;

namespace Bnoerj { namespace Audio {

	ref class AudioEngine;

	public ref struct AudioCategory : IEquatable<AudioCategory^>
	{
		AudioEngine^ engine;
		String^ name;
		UInt16 category;

	internal:
		AudioCategory(AudioEngine^ engine, String^ name);

	public:
		property String^ Name { String^ get(); }

		void Pause();
		void Resume();
		void Stop(AudioStopOptions options);

		void SetVolume(float volume);

		virtual bool Equals(AudioCategory^ other);
		virtual bool Equals(Object^ other) override;
		virtual int GetHashCode() override;

		static bool operator ==(AudioCategory^ a, AudioCategory^ b);
		static bool operator !=(AudioCategory^ a, AudioCategory^ b);
	};
}}
