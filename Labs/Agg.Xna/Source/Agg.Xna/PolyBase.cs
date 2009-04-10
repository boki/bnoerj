// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna
{
	/// <summary>
	/// Defines constants to determine the subpixel accuracy.
	/// </summary>
	/// <remarks>
	/// These constants determine the subpixel accuracy, to be more precise,
	/// the number of bits of the fractional part of the coordinates.
	/// The possible coordinate capacity in bits can be calculated by formula:
	/// sizeof(int) * 8 - poly_base_shift * 2, i.e, for 32-bit integers and
	/// 8-bits fractional part the capacity is 16 bits or [-32768..32767].
	/// </remarks>
	public static class PolyBase
	{
		/// <summary>
		/// The shift of the fractional part.
		/// </summary>
		public const int Shift = 8;

		/// <summary>
		/// The size of the fractional part.
		/// </summary>
		public const int Size = 1 << Shift;

		/// <summary>
		/// The mask of the fractional part.
		/// </summary>
		public const int Mask = Size - 1;

		/// <summary>
		/// The minimum value.
		/// </summary>
		public const int MinValue = -0x7FFFFFFF;

		/// <summary>
		/// The maximum value.
		/// </summary>
		public const int MaxValue = 0x7FFFFFFF;

		/// <summary>
		/// Converts a floating-point number to the fixed-point representation.
		/// </summary>
		/// <param name="c">The floating-point number to convert.</param>
		/// <returns>The fixed-point number representation of the floating-point number.</returns>
		public static int PolyCoordinate(double number)
		{
			return (int)(number * PolyBase.Size);
		}
	}
}
