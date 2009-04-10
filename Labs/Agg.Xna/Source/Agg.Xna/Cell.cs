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
	using System.Runtime.InteropServices;

	/// <summary>
	/// The Cell structure.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Cell : IComparable<Cell>
	{
		/// <summary>
		/// The x-coordinate of the cell.
		/// </summary>
		public short X;

		/// <summary>
		/// The y-coordinate of the cell.
		/// </summary>
		public short Y;

		/// <summary>
		/// The packed x- and y-coordinates of the cell.
		/// </summary>
		public int PackedCoord;

		/// <summary>
		/// The coverage of the cell.
		/// </summary>
		public int Cover;

		/// <summary>
		/// The area of the cell.
		/// </summary>
		public int Area;

		/// <summary>
		/// Sets the coverage and area of the cell.
		/// </summary>
		/// <param name="cover">The new coverage.</param>
		/// <param name="area">The new area.</param>
		public void SetCover(int cover, int area)
		{
			Cover = cover;
			Area = area;
		}

		/// <summary>
		/// Adds coverage and area to the cells current values.
		/// </summary>
		/// <param name="cover">The coverage to add.</param>
		/// <param name="area">The area to add.</param>
		public void AddCover(int cover, int area)
		{
			Cover += cover;
			Area += area;
		}

		/// <summary>
		/// Sets the coordinate of the cell.
		/// </summary>
		/// <param name="x">The new x-coordinate.</param>
		/// <param name="y">The new y-coordinate.</param>
		public void SetCoord(int x, int y)
		{
			X = (short)x;
			Y = (short)y;
			PackedCoord = (y << 16) + x;
		}

		/// <summary>
		/// Sets the coordinate, coverage and area of the cell.
		/// </summary>
		/// <param name="x">The new x-coordinate.</param>
		/// <param name="y">The new y-coordinate.</param>
		/// <param name="cover">The new coverage.</param>
		/// <param name="area">The new area.</param>
		public void Set(int x, int y, int cover, int area)
		{
			X = (short)x;
			Y = (short)y;
			PackedCoord = (y << 16) + x;
			Cover = cover;
			Area = area;
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the
		/// objects being compared. The return value has the following meanings:
		/// Less than zero: This object is less than the other parameter.
		/// Zero: This object is equal to other.
		/// Greater than zero: This object is greater than other.
		/// </returns>
		public int CompareTo(Cell other)
		{
			return PackedCoord - other.PackedCoord;
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			return String.Format("{{X:{1}, Y:{2}, Packed:{0}, Cover:{3}, Area:{4}}}", PackedCoord, X, Y, Cover, Area);
		}
	}
}
