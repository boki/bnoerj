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
	/// The IRenderer interface defines common methods and properties for
	/// rendering.
	/// </summary>
	public interface IRenderer
	{
		/// <summary>
		/// Renders the specified scanline in the specified color.
		/// </summary>
		/// <param name="scanline">The scanline to render.</param>
		/// <param name="color">The color to render the scanline in.</param>
		void Render(Scanline scanline, Color color);
	}
}
