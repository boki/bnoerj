// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

#include "NativeAudioObject.h"

namespace Bnoerj { namespace Audio { namespace Native {

	ref class WaveBank : public AudioObject
	{
		HANDLE hStreamingWaveBankFile;

	public:
		WaveBank(IXACT3Engine* pEngine, String^ filename);
		WaveBank(IXACT3Engine* pEngine, String^ filename, DWORD offset, short packetSize);

		virtual void Release() override;

		DWORD GetStatus();
	};

}}}
