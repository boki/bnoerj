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
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Contains commonly used precalculated values.
	/// </summary>
	public static class MathHelper
	{
		/// <summary>
		/// Retrieves a value indicating whether the two double values can be
		/// considered equal using the specified epsilon.
		/// </summary>
		/// <param name="v1">The first value to compare.</param>
		/// <param name="v2">The second value to compare.</param>
		/// <param name="epsilon">The epsilon value.</param>
		/// <returns>true if the two values are close; false otherwise.</returns>
		public static bool IsEqual(double v1, double v2, double epsilon)
		{
			return Math.Abs(v1 - v2) <= epsilon;
		}

		/// <summary>
		/// Calculates the distance of the two points squared.
		/// </summary>
		/// <param name="x1">The x-coordinate of the first point.</param>
		/// <param name="y1">The y-coordinate of the first point.</param>
		/// <param name="x2">The x-coordinate of the second point.</param>
		/// <param name="y2">The y-coordinate of the second point.</param>
		/// <returns></returns>
		public static double DistanceSquared(double x1, double y1, double x2, double y2)
		{
			double dx = x2 - x1;
			double dy = y2 - y1;
			return dx * dx + dy * dy;
		}
	}
}
