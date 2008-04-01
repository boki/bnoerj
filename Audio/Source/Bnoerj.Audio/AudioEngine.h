// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

#include "NativeEngine.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;

namespace Bnoerj { namespace Audio {

	public ref class AudioEngine
	{
		static Dictionary<IntPtr, WeakReference^>^ audioInstances;
		static bool hookedCueDestroy;

		bool isDisposed;

	internal:
		static Object^ syncRoot;

		Native::Engine^ engine;
		IXACT3Engine* pEngine;

	private:
		static AudioEngine()
		{
			syncRoot = gcnew Object();
			audioInstances = gcnew Dictionary<IntPtr, WeakReference^>();
			hookedCueDestroy = false;
		}

	public:
		AudioEngine(String^ settingsFile);
		AudioEngine(String^ settingsFile, TimeSpan lookAheadTime, Guid rendererId);

		~AudioEngine();

		property bool IsDisposed
		{
			bool get();
		}

		property ReadOnlyCollection<RendererDetail^>^ RendererDetails
		{
			ReadOnlyCollection<RendererDetail^>^ get();
		}

		event EventHandler^ Disposing;

		AudioCategory^ GetCategory(String^ name);

		float GetGlobalVariable(String^ name);
		void SetGlobalVariable(String^ name, float value);

		void Update();

	protected:
		!AudioEngine();

		void Initialize(String^ settingsFile, TimeSpan lookAheadTime, Guid rendererId);

	internal:
		static void AddAudioInstance(void* ptr, Object^ instance);
		static void RemoveAudioInstance(void* ptr);

		static void NotifyCueDestroyed(IntPtr ptrCue);
		static void NotifyEngineDestroyed(AudioEngine^ engine);
	};
}}
