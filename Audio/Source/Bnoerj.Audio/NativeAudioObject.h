// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

namespace Bnoerj { namespace Audio { namespace Native {

	ref class AudioObject abstract
	{
	internal:
		IXACT3Engine* pEngine;
		void* pObject;
		void* pData;

		AudioObject()
			: pEngine(NULL)
			, pObject(NULL)
			, pData(NULL)
		{}
		AudioObject(IXACT3Engine* pEngine, void* pObject)
			: pEngine(pEngine)
			, pObject(pObject)
			, pData(NULL)
		{}
		AudioObject(IXACT3Engine* pEngine, void* pObject, void* pData)
			: pEngine(pEngine)
			, pObject(pObject)
			, pData(pData)
		{}

	public:
		virtual void Release() = 0;
	};

}}}
