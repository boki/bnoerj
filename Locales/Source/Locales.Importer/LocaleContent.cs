// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Text;
using Bnoerj.Locales.Importer.Fonts;
using Bnoerj.Locales.Importer.KeyboardLayouts;

namespace Bnoerj.Locales.Importer
{
	class LocaleContent
	{
		CultureInfo cultureInfo;
		KeyboardLayoutContent keyboardLayout;
		FontContent font;

		internal LocaleContent(CultureInfo cultureInfo, KeyboardLayoutContent keyboardLayout, FontContent font)
		{
			this.cultureInfo = cultureInfo;
			this.keyboardLayout = keyboardLayout;
			this.font = font;
		}

		internal CultureInfo CultureInfo
		{
			get { return cultureInfo; }
		}

		internal KeyboardLayoutContent KeyboardLayout
		{
			get { return keyboardLayout; }
		}

		internal FontContent Font
		{
			get { return font; }
		}
	}
}
