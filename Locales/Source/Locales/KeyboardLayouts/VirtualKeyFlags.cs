// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;

namespace Bnoerj.Locales.KeyboardLayouts
{
	[Flags]
	public enum VirtualKeyFlags
	{
		/// <summary>The VirtualKeyValue is empty</summary>
		Empty,
		/// <summary>Swiss German</summary>
		SGCaps,
		/// <summary></summary>
		CapsEqualToShift,
		/// <summary></summary>
		AltGrCapsEqualToAltGrShift,
		/// <summary></summary>
		SpecialGrCapsEqualToSpecialShift
	}
}
