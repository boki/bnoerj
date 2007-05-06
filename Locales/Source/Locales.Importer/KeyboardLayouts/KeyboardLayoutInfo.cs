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
	internal struct KeyboardLayoutInfo
	{
		internal uint HKL;
		internal String LayoutFile;
		internal String LayoutText;
		internal String LayoutName;
		internal ushort LayoutID;
	}
}
