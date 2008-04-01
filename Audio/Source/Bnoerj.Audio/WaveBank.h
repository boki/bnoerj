// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;

namespace Bnoerj { namespace Audio {

	public ref class WaveBank : public AudioObject
	{
		System::IO::FileStream^ stream;

	public:
		WaveBank(AudioEngine^ engine, String^ nonStreamingWaveBankFilename);
		WaveBank(AudioEngine^ engine, String^ streamingWaveBankFilename, int offset, int packetSize);

		property bool IsPrepared { bool get(); }
		property bool IsInUse { bool get(); }
	};
}}
