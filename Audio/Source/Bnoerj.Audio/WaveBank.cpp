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
#include "AudioObject.h"
#include "WaveBank.h"

#include "NativeWaveBank.h"

using namespace System::IO;
using namespace Bnoerj::Audio;

WaveBank::WaveBank(AudioEngine^ engine, String^ nonStreamingWaveBankFilename)
{
	if (engine == nullptr)
	{
		throw gcnew ArgumentNullException("engine", StringResources::NullNotAllowed);
	}
	if (String::IsNullOrEmpty(nonStreamingWaveBankFilename) == true)
	{
		throw gcnew ArgumentNullException("nonStreamingWaveBankFilename", StringResources::NullNotAllowed);
	}

	nativeObject = gcnew Native::WaveBank(engine->pEngine, nonStreamingWaveBankFilename);
	engine->AddAudioInstance(nativeObject->pObject, this);

	this->engine = engine;
}

WaveBank::WaveBank(AudioEngine^ engine, String^ streamingWaveBankFilename, int offset, int packetSize)
{
	if (engine == nullptr)
	{
		throw gcnew ArgumentNullException("engine", StringResources::NullNotAllowed);
	}
	if (String::IsNullOrEmpty(streamingWaveBankFilename) == true)
	{
		throw gcnew ArgumentNullException("streamingWaveBankFilename", StringResources::NullNotAllowed);
	}

	nativeObject = gcnew Native::WaveBank(engine->pEngine, streamingWaveBankFilename, offset, (short)packetSize);
	engine->AddAudioInstance(nativeObject->pObject, this);

	this->engine = engine;
}

bool WaveBank::IsPrepared::get()
{
	DWORD status = static_cast<Native::WaveBank^>(nativeObject)->GetStatus();
	return (status & XACT_WAVEBANKSTATE_PREPARED) != 0;
}

bool WaveBank::IsInUse::get()
{
	DWORD status = static_cast<Native::WaveBank^>(nativeObject)->GetStatus();
	return (status & XACT_WAVEBANKSTATE_INUSE) != 0;
}
