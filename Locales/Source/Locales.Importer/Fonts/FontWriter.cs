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
using Bnoerj.Locales.Text;

namespace Bnoerj.Locales.Importer.Fonts
{
	[ContentTypeWriter]
	class FontWriter : ContentTypeWriter<FontContent>
	{
		protected override void Write(ContentWriter output, FontContent value)
		{
			output.WriteObject(value.Texture);
			output.Write(value.Height);

			output.Write(value.Glyphs.Count);
			foreach (KeyValuePair<char, Glyph> charGlyph in value.Glyphs)
			{
				output.Write(charGlyph.Key);
				output.WriteObject(charGlyph.Value.Bounds);
			}

			output.Write(value.KerningPairs.Count);
			foreach (KeyValuePair<uint, int> pair in value.KerningPairs)
			{
				output.Write(pair.Key);
				output.Write(pair.Value);
			}
		}

		public override String GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof(FontReader).AssemblyQualifiedName;
		}
	}
}
