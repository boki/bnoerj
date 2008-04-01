// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;

namespace Bnoerj { namespace Audio {

	public ref class AudioListener
	{
	internal:
		X3DAUDIO_LISTENER* listenerData;

		//FIXME: needs to be non-IDisposable
		~AudioListener();
		!AudioListener();

	public:
		AudioListener();

		property XnaVector3 Position
		{
			XnaVector3 get();
			void set(XnaVector3 value);
		}

		property XnaVector3 Forward
		{
			XnaVector3 get();
			void set(XnaVector3 value);
		}

		property XnaVector3 Up
		{
			XnaVector3 get();
			void set(XnaVector3 value);
		}

		property XnaVector3 Velocity
		{
			XnaVector3 get();
			void set(XnaVector3 value);
		}
	};
}}
