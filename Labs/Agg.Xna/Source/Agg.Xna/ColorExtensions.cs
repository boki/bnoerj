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
	/// Provides extensions methods for the Microsoft.Xna.Framework.Graphics.Color
	/// structure.
	/// </summary>
	public static class ColorExtensions
	{
		/// <summary>
		/// Linearly interpolates between two colors.
		/// </summary>
		/// <param name="self">Start color.</param>
		/// <param name="color">End color.</param>
		/// <param name="amount">A value between 0 and 1.0 indicating the weight of color.</param>
		/// <returns>The interpolated color.</returns>
		public static Color Gradient(this Color self, Color color, double amount)
		{
			// REVIEW: replace with Color.Lerp?
			Color result = new Color();
			int ia = (int)(amount * 256);
			result.R = (byte)(self.R + (((color.R - self.R) * ia) >> 8));
			result.G = (byte)(self.G + (((color.G - self.G) * ia) >> 8));
			result.B = (byte)(self.B + (((color.B - self.B) * ia) >> 8));
			result.A = (byte)(self.A + (((color.A - self.A) * ia) >> 8));
			return result;
		}
	}
}
