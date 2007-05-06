// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;

namespace Bnoerj.Locales.Importer.KeyboardLayouts
{
	class KeyboardLayoutSource
	{
		String layoutId;

		internal KeyboardLayoutSource(String layoutId)
		{
			this.layoutId = layoutId;
		}

		internal String LayoutId
		{
			get { return layoutId; }
		}
	}
}
