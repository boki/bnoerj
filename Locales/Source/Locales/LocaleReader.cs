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
using Microsoft.Xna.Framework.Content;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Text;

namespace Bnoerj.Locales
{
	public class LocaleReader : ContentTypeReader<Locale>
	{
		protected override Locale Read(ContentReader input, Locale existingInstance)
		{
			int lcid = input.ReadInt32();
			CultureInfo ci = new CultureInfo(lcid);
			KeyboardLayout keyboaryLayout = input.ReadObject<KeyboardLayout>();
			Font font = input.ReadObject<Font>();
			return new Locale(ci, keyboaryLayout, font);
		}
	}
}
