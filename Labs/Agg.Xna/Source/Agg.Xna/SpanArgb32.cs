﻿// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna
{
	using Microsoft.Xna.Framework.Graphics;

	/// <summary>
	/// Supports rendering/reading to/from a rendering buffer whose pixels are
	/// stored in ARGB order and have a size of 32 bits.
	/// </summary>
	public class SpanArgb32 : ISpan
	{
		public void Render(RowIterator row, int x, int count, CoverageIterator covers, Color c)
		{
			row.Move(x << 2);
			for (int i = 0; i < count << 2; i += 4)
			{
				int alpha = covers.Current * c.A;
				int a = row[i + 0];
				int r = row[i + 1];
				int g = row[i + 2];
				int b = row[i + 3];
				row[i + 0] = (byte)((((c.A - a) * alpha) + (a << 16)) >> 16);
				row[i + 1] = (byte)((((c.R - r) * alpha) + (r << 16)) >> 16);
				row[i + 2] = (byte)((((c.G - g) * alpha) + (g << 16)) >> 16);
				row[i + 3] = (byte)((((c.B - b) * alpha) + (b << 16)) >> 16);

				covers.MoveNext();
			}
		}

		public void HorizontalLine(RowIterator row, int x, int count, Color c)
		{
			row.Move(x << 2);
			for (int i = 0; i < count << 2; i += 4)
			{
				row[i + 0] = c.A;
				row[i + 1] = c.R;
				row[i + 2] = c.G;
				row[i + 3] = c.B;
			}
		}

		public Color Get(RowIterator row, int x)
		{
			row.Move(x << 2);
			Color c = new Color();
			c.A = row[0];
			c.R = row[1];
			c.G = row[2];
			c.B = row[3];
            return c;
		}
	}
}
