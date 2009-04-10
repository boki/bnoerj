// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Agg.Xna.Curves
{
	/// <summary>
	/// Defines the curve approximation methods.
	/// </summary>
	public enum CurveApproximationMethod
	{
		/// <summary>
		/// Approximate the curve incremental.
		/// </summary>
        Incremental,

		/// <summary>
		/// Approximate the curve divisional.
		/// </summary>
        Divisional
	}
}
