// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;
using namespace System::Runtime::Serialization;

namespace Bnoerj { namespace Audio {

	[Serializable]
	public ref class NoAudioHardwareException sealed : public ExternalException
	{
		// Methods
	public:
		NoAudioHardwareException()
		{}
		NoAudioHardwareException(String^ message)
			: ExternalException(message)
		{}
		NoAudioHardwareException(SerializationInfo^ info, StreamingContext context)
			: ExternalException(info, context)
		{}
		NoAudioHardwareException(String^ message, Exception^ inner)
			: ExternalException(message, inner)
		{}
	};

}};
