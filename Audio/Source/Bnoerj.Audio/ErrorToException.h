// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

#include "NoAudioHardwareException.h"
#include "StringResources.h"

using namespace System;
using namespace System::IO;

namespace Bnoerj { namespace Audio { namespace Native {

	class ErrorToException
	{
	public:

		static void Throw(HRESULT hr)
		{
			switch (hr)
			{
			case XACTENGINE_E_OUTOFMEMORY:
				// Out of memory
				throw gcnew OutOfMemoryException();
			case XACTENGINE_E_INVALIDARG:
				// Invalid arg
				throw gcnew ArgumentException();
			case XACTENGINE_E_NOTIMPL:
				// Not implemented
				throw gcnew NotImplementedException();
			case XACTENGINE_E_FAIL:
				// Unknown error
				throw gcnew Exception();

			case XACTENGINE_E_ALREADYINITIALIZED:
				// The engine is already initialized
				throw gcnew InvalidOperationException(StringResources::AlreadyInitialized);
			case XACTENGINE_E_NOTINITIALIZED:
				// The engine has not been initialized
				throw gcnew InvalidOperationException(StringResources::NotInitialized);
			case XACTENGINE_E_EXPIRED:
				// The engine has expired (demo or pre-release version)
				throw gcnew InvalidOperationException(StringResources::EngineExpired);
			case XACTENGINE_E_NONOTIFICATIONCALLBACK:
				// No notification callback
				throw gcnew Exception(StringResources::NoNotificationCallback);
			case XACTENGINE_E_NOTIFICATIONREGISTERED:
				// Notification already registered
				throw gcnew Exception(StringResources::NotificationRegistered);
			case XACTENGINE_E_INVALIDUSAGE:
				// Invalid usage
				throw gcnew InvalidOperationException();
			case XACTENGINE_E_INVALIDDATA:
				// Invalid data
				throw gcnew ArgumentException(StringResources::InvalidData);
			case XACTENGINE_E_INSTANCELIMITFAILTOPLAY:
				// Fail to play due to instance limit
				throw gcnew Exception(StringResources::InstanceLimitFailToPlay);
			case XACTENGINE_E_NOGLOBALSETTINGS:
				// Global Settings not loaded
				throw gcnew Exception(StringResources::NoGlobalSettings);
			case XACTENGINE_E_INVALIDVARIABLEINDEX:
				// Invalid variable index
				throw gcnew Exception(StringResources::InvalidVariableIndex);
			case XACTENGINE_E_INVALIDCATEGORY:
				// Invalid category
				throw gcnew IndexOutOfRangeException(StringResources::InvalidCategory);
			case XACTENGINE_E_INVALIDCUEINDEX:
				// Invalid cue index
				throw gcnew IndexOutOfRangeException(StringResources::InvalidCue);
			case XACTENGINE_E_INVALIDWAVEINDEX:
				// Invalid wave index
				throw gcnew IndexOutOfRangeException(StringResources::InvalidWaveIndex);
			case XACTENGINE_E_INVALIDTRACKINDEX:
				// Invalid track index
				throw gcnew IndexOutOfRangeException(StringResources::InvalidTrackIndex);
			case XACTENGINE_E_INVALIDSOUNDOFFSETORINDEX:
				// Invalid sound offset or index
				throw gcnew IndexOutOfRangeException(StringResources::InvalidSoundoffsetOrIndex);
			case XACTENGINE_E_READFILE:
				// Error reading a file
				throw gcnew IOException();
			case XACTENGINE_E_UNKNOWNEVENT:
				// Unknown event type
				throw gcnew Exception(StringResources::UnknownEvent);
			case XACTENGINE_E_INCALLBACK:
				// Invalid call of method of function from callback
				throw gcnew InvalidOperationException(StringResources::InCallback);
			case XACTENGINE_E_NOWAVEBANK:
				// No wavebank exists for desired operation
				throw gcnew InvalidOperationException(StringResources::NoWaveBank);
			case XACTENGINE_E_SELECTVARIATION:
				// Unable to select a variation
				throw gcnew InvalidOperationException(StringResources::SelectVariation);
			case XACTENGINE_E_MULTIPLEAUDITIONENGINES:
				// There can be only one audition engine
				throw gcnew Exception(StringResources::MultipleAuditionEngines);
			case XACTENGINE_E_WAVEBANKNOTPREPARED:
				// The wavebank is not prepared
				throw gcnew InvalidOperationException(StringResources::WaveBankNotPrepared);
			case XACTENGINE_E_NORENDERER:
				// No audio device found on.
				throw gcnew NoAudioHardwareException();
			case XACTENGINE_E_INVALIDENTRYCOUNT:
				// Invalid entry count for channel maps
				throw gcnew ArgumentException(StringResources::InvalidEntryCount);
			case XACTENGINE_E_SEEKTIMEBEYONDCUEEND:
				// Time offset for seeking is beyond the cue end.
			//XACTENGINE_E_SEEKTIMEBEYONDWAVEEND:
				// Time offset for seeking is beyond the wave end.
				throw gcnew Exception(StringResources::SeekTimeBeyondEnd);
			case XACTENGINE_E_NOFRIENDLYNAMES:
				// Friendly names are not included in the bank.
				throw gcnew Exception(StringResources::NoFriendlyNames);

			case XACTENGINE_E_AUDITION_WRITEFILE:
				// Error writing a file during auditioning
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_NOSOUNDBANK:
				// Missing a soundbank
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_INVALIDRPCINDEX:
				// Missing an RPC curve
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_MISSINGDATA:
				// Missing data for an audition command
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_UNKNOWNCOMMAND:
				// Unknown command
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_INVALIDDSPINDEX:
				// Missing a DSP parameter
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_MISSINGWAVE:
				// Wave does not exist in auditioned wavebank
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_CREATEDIRECTORYFAILED:
				// Failed to create a directory for streaming wavebank data
				throw gcnew Exception();
			case XACTENGINE_E_AUDITION_INVALIDSESSION:
				// Invalid audition session
				throw gcnew Exception();
			}
			throw gcnew InvalidOperationException(StringResources::UnexpectedError);
		}
	};

}}}
