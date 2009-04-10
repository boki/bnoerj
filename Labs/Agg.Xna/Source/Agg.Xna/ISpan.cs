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
	/// Supports rendering into a rendering buffer.
	/// TODO: Converts from the  internal Color to the rendering buffers format
	/// </summary>
	public interface ISpan
	{
		/// <summary>
		/// Renders the specified coverage iterator into the rendering buffer.
		/// </summary>
		/// <param name="row">The row of the output buffer to render into.</param>
		/// <param name="x">The starting x-coordinate.</param>
		/// <param name="count">The number of pixels to render.</param>
		/// <param name="covers">The coverage information to use.</param>
		/// <param name="color">The color to render.</param>
		void Render(RowIterator row, int x, int count, CoverageIterator covers, Color color);

		/// <summary>
		/// Renders a horizontal line into the rendering buffer.
		/// </summary>
		/// <param name="row">The row of the output buffer to render into.</param>
		/// <param name="x">The starting x-coordinate.</param>
		/// <param name="count">The number of pixels to render.</param>
		/// <param name="color">The color to render.</param>
		void HorizontalLine(RowIterator row, int x, int count, Color color);

		/// <summary>
		/// Retrieves the color of the rendering buffer in the specified row at
		/// the specified x-coordinate.
		/// </summary>
		/// <param name="row">The row to retrieve</param>
		/// <param name="x">The x-coordinate.</param>
		/// <returns>The color at the specified x-coordinate in the row.</returns>
		Color Get(RowIterator row, int x);
	}
}
