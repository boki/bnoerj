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

namespace Bnoerj.Locales.Text
{
	/// <summary>
	/// The Glyph class
	/// </summary>
	public struct Glyph
	{
		public Rectangle Bounds;

		public Point Position
		{
			get { return new Point(Bounds.X, Bounds.Y); }
		}

		public Point Size
		{
			get { return new Point(Bounds.Width, Bounds.Height); }
		}

		internal Glyph(Rectangle bounds)
		{
			Bounds = bounds;
		}
	}
}
