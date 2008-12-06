// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Importer.KeyboardLayouts;
using Bnoerj.Locales.Importer.Fonts;

namespace Bnoerj.Locales.Importer
{
	[ContentTypeWriter]
	class LocaleWriter : ContentTypeWriter<LocaleContent>
	{
		protected override void Write(ContentWriter output, LocaleContent value)
		{
			// REVIEW: Write name?
			output.Write(value.CultureInfo.LCID);
			output.WriteObject<KeyboardLayoutContent>(value.KeyboardLayout);
			output.WriteObject<FontContent>(value.Font);
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof(LocaleReader).AssemblyQualifiedName;
		}
	}
}
