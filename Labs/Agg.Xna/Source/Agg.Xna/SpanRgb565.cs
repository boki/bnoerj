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
	/// stored in RGB order and have a size of 16 bits.
	/// </summary>
	public class SpanRgb565 : ISpan
	{
		static ushort ToRgb565(byte r, byte g, byte b)
		{
			return (ushort)(((r & 0xF8) << 8) | ((g & 0xFC) << 3) | (b >> 3));
		}

		public void Render(RowIterator row, int x, int count, CoverageIterator covers, Color c)
		{
			row.Move(x << 1);
			for (int i = 0; i < count << 1; i += 2)
			{
				ushort rgb = (ushort)(row[i + 0] | row[i + 1]);
				int alpha = covers.Current * c.A;

				int r = (rgb >> 8) & 0xF8;
				int g = (rgb >> 3) & 0xFC;
				int b = (rgb << 3) & 0xF8;

				int v =
					(((((c.R - r) * alpha) + (r << 16)) >> 8) & 0xF800) |
					(((((c.G - g) * alpha) + (g << 16)) >> 13) & 0x7E0) |
					((((c.B - b) * alpha) + (b << 16)) >> 19);

				row[i + 0] = (byte)(v >> 8);
				row[i + 1] = (byte)(v & 0xFF);

				covers.MoveNext();
			}
		}

		public void HorizontalLine(RowIterator row, int x, int count, Color c)
		{
			row.Move(x << 1);
			ushort v = ToRgb565(c.R, c.G, c.B);
			for (int i = 0; i < count << 1; i += 2)
			{
				row[i + 0] = (byte)(v >> 8);
				row[i + 1] = (byte)(v & 0xFF);
			}
		}

		public Color Get(RowIterator row, int x)
		{
			row.Move(x << 1);
			ushort rgb = (ushort)(row[0] | row[1]);
			Color c = new Color();
			c.R = (byte)((rgb >> 8) & 0xF8);
			c.G = (byte)((rgb >> 3) & 0xFC);
			c.B = (byte)((rgb << 3) & 0xF8);
			c.A = 255;
			return c;
		}
	}
}
