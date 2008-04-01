// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

using namespace System;

namespace Bnoerj { namespace Audio {

	public ref struct RendererDetail
	{
		String^ friendlyName;
		Guid guid;

	internal:
		RendererDetail(String^ friendlyName, Guid guid);

	public:

		property String^ FriendlyName
		{
			String^ get();
		}

		property Guid RendererId
		{
			Guid get();
		}

		virtual bool Equals(Object^ other) override;
		virtual int GetHashCode() override;

		static bool operator ==(RendererDetail^ a, RendererDetail^ b);
		static bool operator !=(RendererDetail^ a, RendererDetail^ b);
	};
}}
