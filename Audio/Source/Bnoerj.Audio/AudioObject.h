// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;

#include "NativeAudioObject.h"

namespace Bnoerj { namespace Audio {

	public ref class AudioObject abstract
	{
	internal:
		bool isDisposed;

		AudioEngine^ engine;
		Native::AudioObject^ nativeObject;

		AudioObject()
			: engine(nullptr)
			, nativeObject(nullptr)
		{}
		AudioObject(AudioEngine^ engine, Native::AudioObject^ nativeObject)
			: engine(engine)
			, nativeObject(nativeObject)
		{
			if (engine == nullptr || nativeObject == nullptr || nativeObject->pObject == nullptr)
			{
				throw gcnew InvalidOperationException(StringResources::CouldNotCreateResource);
			}
		}

	public:

		property bool IsDisposed
		{
			bool get() { return isDisposed; }
		}

		event EventHandler^ Disposing;

		~AudioObject()
		{
			msclr::lock lock(AudioEngine::syncRoot);

			// Save dispose state to raise the disposed event properly
			bool wasDisposed = isDisposed;

			lock.release();

			this->!AudioObject();

			if (wasDisposed == false)
			{
				Disposing(this, EventArgs::Empty);
			}

			GC::SuppressFinalize(this);
		}
		!AudioObject()
		{
			msclr::lock lock(AudioEngine::syncRoot);

			if (isDisposed == false)
			{
				isDisposed = true;
				if (nativeObject != nullptr)
				{
					AudioEngine::RemoveAudioInstance(nativeObject->pObject);
					if (engine->IsDisposed == false)
					{
						nativeObject->Release();
					}
					delete nativeObject;
					nativeObject = nullptr;
				}
			}
		}
	};

}}
