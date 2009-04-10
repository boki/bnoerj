// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna.Paths
{
	/// <summary>
	/// Defines path commands.
	/// </summary>
	public enum PathCommand
	{
		/// <summary>
		/// Stops a path.
		/// </summary>
		Stop,

		/// <summary>
		/// Moves the postion.
		/// </summary>
		MoveTo,

		/// <summary>
		/// Draws a line from the current position to the end pont.
		/// </summary>
		LineTo,
	}
}
