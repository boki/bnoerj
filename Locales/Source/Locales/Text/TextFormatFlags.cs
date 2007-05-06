// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;

namespace Bnoerj.Locales.Text
{
	[Flags]
	public enum TextFormatFlags
	{
		Top						= 0x00000000,
		Left					= 0x00000000,
		HorizontalCenter		= 0x00000001,
		Right					= 0x00000002,
		VerticalCenter			= 0x00000004,
		Bottom					= 0x00000008,
		WordBreak				= 0x00000010,
		SingleLine				= 0x00000020,
		ExpandTabs				= 0x00000040,
		NoClipping				= 0x00000100,
		TextBoxControl          = 0x00002000,
		EndEllipsis				= 0x00008000,
		RightToLeft				= 0x00020000,
		WordEllipsis			= 0x00040000,
		GlyphOverhangPadding	= 0x01000000,
		LeftAndRightPadding		= 0x02000000,
		NoPadding				= 0x04000000,

		Default					= Top | Left,
	}
}
