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

	delegate void CueDestroyedEventHandler(IntPtr ptrCue);

	ref class Engine : public AudioObject
	{
		static CueDestroyedEventHandler^ _CueDestroyed;

		UINT destinationChannelCount;
		BYTE* p3DAudioData;
		FLOAT32* pDelayTimes;
		FLOAT32* pMatrixCoefficients;
		X3DAUDIO_DSP_SETTINGS* pDsp;

	internal:
		static Object^ syncRoot;

		static event CueDestroyedEventHandler^ CueDestroyed
		{
		internal:
			void add(CueDestroyedEventHandler^ handler)
			{
				_CueDestroyed += handler;
			}
			void remove(CueDestroyedEventHandler^ handler)
			{
				_CueDestroyed -= handler;
			}
			void raise(IntPtr ptrCue)
			{
				if (_CueDestroyed)
				{
					_CueDestroyed(ptrCue);
				}
			}
		}

	private:
		static Engine()
		{
			syncRoot = gcnew Object();
		}

	public:
		Engine(String^ settingsFilename, unsigned int lookAheadTime, Guid guid);
		virtual void Release() override;

		int GetRendererCount();
		void GetRendererDetail(int index, String^% friendlyName, String^% guid);

		float GetGlobalVariable(String^ name);
		void SetGlobalVariable(String^ name, float value);

		XACTCATEGORY GetCategory(String^ name);

		void Update();

		void Pause(XACTCATEGORY cateorgy, BOOL pause);
		void Stop(XACTCATEGORY cateorgy, DWORD options);

		void SetVolume(XACTCATEGORY cateorgy, float volume);

		void Apply3D(IXACT3Cue* pCue, X3DAUDIO_LISTENER* pListener, X3DAUDIO_EMITTER* pEmitter);
	};

}}}
