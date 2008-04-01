// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;
using namespace System::Globalization;
using namespace System::Resources;

namespace Bnoerj { namespace Audio {

	ref class StringResources
	{
		static CultureInfo^ resCulture = CultureInfo::CurrentCulture;
		static ResourceManager^ resMan;

		property static ResourceManager^ ResMan
		{
			ResourceManager^ get()
			{
				if (resMan == nullptr)
				{
					ResourceManager^ rm = gcnew ResourceManager("Bnoerj.Audio.StringResources", StringResources::typeid->Assembly);
					resMan = rm;
				}
				return resMan;
			}
		}

	internal:

#define StringResourceGetterImpl(Name) \
	property static String^ Name \
	{ \
		String^ get() { return ResMan->GetString(#Name, resCulture); } \
	}

		StringResourceGetterImpl(UnexpectedError)
		StringResourceGetterImpl(NullNotAllowed)
		StringResourceGetterImpl(CouldNotCreateResource)

		StringResourceGetterImpl(InvalidEmitterDopplerScale)
		StringResourceGetterImpl(Apply3DBeforePlaying)

		StringResourceGetterImpl(AlreadyInitialized)
		StringResourceGetterImpl(NotInitialized)
		StringResourceGetterImpl(EngineExpired)
		StringResourceGetterImpl(NoNotificationCallback)
		StringResourceGetterImpl(NotificationRegistered)
		StringResourceGetterImpl(InvalidData)
		StringResourceGetterImpl(InstanceLimitFailToPlay)
		StringResourceGetterImpl(NoGlobalSettings)
		StringResourceGetterImpl(InvalidVariableIndex)
		StringResourceGetterImpl(InvalidCategory)
		StringResourceGetterImpl(InvalidCue)
		StringResourceGetterImpl(InvalidWaveIndex)
		StringResourceGetterImpl(InvalidTrackIndex)
		StringResourceGetterImpl(InvalidSoundoffsetOrIndex)
		StringResourceGetterImpl(UnknownEvent)
		StringResourceGetterImpl(InCallback)
		StringResourceGetterImpl(NoWaveBank)
		StringResourceGetterImpl(SelectVariation)
		StringResourceGetterImpl(MultipleAuditionEngines)
		StringResourceGetterImpl(WaveBankNotPrepared)
		StringResourceGetterImpl(InvalidEntryCount)
		StringResourceGetterImpl(SeekTimeBeyondEnd)
		StringResourceGetterImpl(NoFriendlyNames)

#undef StringResourceGetterImpl
	};

}}
