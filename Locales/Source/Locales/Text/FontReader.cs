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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bnoerj.Locales.Text
{
	public class FontReader : ContentTypeReader<Font>
	{
		protected override Font Read(ContentReader input, Font existingInstance)
		{
			Texture2D texture = input.ReadObject<Texture2D>();
			float height = input.ReadSingle();

			int glyphCount = input.ReadInt32();
			Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>(glyphCount);
			for (int i = 0; i < glyphCount; i++)
			{
				char c = input.ReadChar();
				Rectangle bounds = input.ReadObject<Rectangle>();
				glyphs.Add(c, new Glyph(bounds));
			}

			int kerningPairCount = input.ReadInt32();
			SortedDictionary<uint, int> kerningPairs = new SortedDictionary<uint, int>();
			for (int i = 0; i < kerningPairCount; i++)
			{
				uint pair = input.ReadUInt32();
				int amount = input.ReadInt32();
				kerningPairs.Add(pair, amount);
			}
			return new Font(texture, height, glyphs, kerningPairs);
		}
	}
}
