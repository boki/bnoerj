// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Bnoerj.Locales.Text;

namespace Bnoerj.Locales.Importer.Fonts
{
	class FontContent
	{
		Texture2DContent texture;
		int height;
		Dictionary<char, Glyph> glyphs;
		SortedDictionary<uint, int> kerningPairs;

		internal FontContent(BitmapContent texture, int height, Dictionary<char, Glyph> glyphs, SortedDictionary<uint, int> kerningPairs)
		{
			this.texture = new Texture2DContent();
			this.texture.Mipmaps = texture;
			this.height = height;
			this.glyphs = glyphs;
			this.kerningPairs = kerningPairs;
		}

		internal Texture2DContent Texture
		{
			get { return texture; }
		}

		internal float Height
		{
			get { return height; }
		}

		internal Dictionary<char, Glyph> Glyphs
		{
			get { return glyphs; }
		}

		internal SortedDictionary<uint, int> KerningPairs
		{
			get { return kerningPairs; }
		}
	}
}
