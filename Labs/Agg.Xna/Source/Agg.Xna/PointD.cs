// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna
{
	using System;

	/// <summary>
	/// Represents an ordered pair of floating-point x- and y-coordinates that
	/// defines a point in a two-dimensional plane.
	/// </summary>
	public struct PointD
	{
		public static PointD Zero = new PointD(0, 0);

		/// <summary>
		/// The x-coordinate of the point.
		/// </summary>
		public double X;

		/// <summary>
		/// The y-coordinate of the point.
		/// </summary>
		public double Y;

		/// <summary>
		/// Initializes a new instance of the PointD structure with the
		/// specified coordinates.
		/// </summary>
		/// <param name="x">The x-coordinate of the point.</param>
		/// <param name="y">The y-coordinate of the point.</param>
		public PointD(double x, double y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return String.Format("{{X:{0}, Y:{1}}}", X, Y);
		}
	}
}
