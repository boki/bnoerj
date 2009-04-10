// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
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
	/// stored in RGB order and have a size of 24 bits.
	/// </summary>
	public class SpanRgb24 : ISpan
	{
		public void Render(RowIterator row, int x, int count, CoverageIterator covers, Color c)
		{
			row.Move(x + x + x);
			for (int i = 0; i < 3 * count; i += 3)
			{
				int alpha = covers.Current * c.A;
				int r = row[i + 0];
				int g = row[i + 1];
				int b = row[i + 2];
				row[i + 0] = (byte)((((c.R - r) * alpha) + (r << 16)) >> 16);
				row[i + 1] = (byte)((((c.G - g) * alpha) + (g << 16)) >> 16);
				row[i + 2] = (byte)((((c.B - b) * alpha) + (b << 16)) >> 16);

				covers.MoveNext();
			}
		}

		public void HorizontalLine(RowIterator row, int x, int count, Color c)
		{
			row.Move(x + x + x);
			for (int i = 0; i < 3 * count; i += 3)
			{
				row[i + 0] = c.R;
				row[i + 1] = c.G;
				row[i + 2] = c.B;
			}
		}

		public Color Get(RowIterator row, int x)
		{
			row.Move(x + x + x);
			Color c = new Color();
			c.R = row[0];
			c.G = row[1];
			c.B = row[2];
			c.A = 255;
			return c;
		}
	}
}
