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

namespace Bnoerj.Locales.Text
{
	public class Font : IDisposable
	{
		Texture2D texture;
		float height;
		Dictionary<char, Glyph> glyphs;
		SortedDictionary<uint, int> kerningPairs;
		bool disposed;

		internal Font(Texture2D texture, float height, Dictionary<char, Glyph> glyphs, SortedDictionary<uint, int> kerningPairs)
		{
			this.texture = texture;
			this.height = height;
			this.glyphs = glyphs;
			this.kerningPairs = kerningPairs;
		}

		public Texture2D Texture
		{
			get { return texture; }
		}

		public float Height
		{
			get { return height; }
		}

		public Dictionary<char, Glyph> Glyphs
		{
			get { return glyphs; }
		}

		public SortedDictionary<uint, int> KerningPairs
		{
			get { return kerningPairs; }
		}

		public bool IsDisposed
		{
			get { return disposed; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (disposed == false)
			{
				if (texture != null && texture.IsDisposed == false)
				{
					texture.Dispose();
				}
			}
			disposed = true;
		}
	}
}
