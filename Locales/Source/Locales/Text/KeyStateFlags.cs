// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;

namespace Bnoerj.Locales.Text
{
	[Flags]
	public enum KeyStateFlags : byte
	{
		Up       = 0x00,
		Down     = 0x01,
		Repeat   = 0x02,
		Released = 0x04,
	}
}
