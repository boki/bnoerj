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
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Bnoerj.Locales.Importer
{
	/// <summary>
	/// 
	/// </summary>
	class LocaleSource
	{
		CultureInfo cultureInfo;
		String fontName;
		String klcFilename;

		internal LocaleSource(CultureInfo cultureInfo, String fontName, String klcFilename)
		{
			this.cultureInfo = cultureInfo;
			this.fontName = fontName;
			this.klcFilename = klcFilename;
		}

		internal CultureInfo CultureInfo
		{
			get { return cultureInfo; }
		}

		internal String FontName
		{
			get { return fontName; }
		}

		internal String KlcFilename
		{
			get { return klcFilename; }
		}
	}
}
