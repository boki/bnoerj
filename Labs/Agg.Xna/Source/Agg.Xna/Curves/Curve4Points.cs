// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna.Curves
{
	/// <summary>
	/// The Curve4Points structure.
	/// </summary>
	public struct Curve4Points
	{
		double[] cp;

		public Curve4Points(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			cp = new double[] {
				x1, y1, x2, y2,
				x3, y3, x4, y4
			};
		}

		public double this[int index]
		{
			get { return cp[index]; }
			set { cp[index] = value; }
		}

		public void Initialize(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			cp = new double[] {
				x1, y1, x2, y2,
				x3, y3, x4, y4
			};
		}

		public static Curve4Points CatromToBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			// Trans. matrix Catmull-Rom to Bezier
			//
			//  0       1       0       0
			//  -1/6    1       1/6     0
			//  0       1/6     1       -1/6
			//  0       0       1       0
			//
			return new Curve4Points(
				x2,
				y2,
				(-x1 + 6 * x2 + x3) / 6,
				(-y1 + 6 * y2 + y3) / 6,
				(x2 + 6 * x3 - x4) / 6,
				(y2 + 6 * y3 - y4) / 6,
				x3,
				y3);
		}

		public static Curve4Points CatromToBezier(Curve4Points cp)
		{
			return CatromToBezier(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
		}

		public static Curve4Points UbSplineToBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			// Trans. matrix Uniform BSpline to Bezier
			//
			//  1/6     4/6     1/6     0
			//  0       4/6     2/6     0
			//  0       2/6     4/6     0
			//  0       1/6     4/6     1/6
			//
			return new Curve4Points(
				(x1 + 4 * x2 + x3) / 6,
				(y1 + 4 * y2 + y3) / 6,
				(4 * x2 + 2 * x3) / 6,
				(4 * y2 + 2 * y3) / 6,
				(2 * x2 + 4 * x3) / 6,
				(2 * y2 + 4 * y3) / 6,
				(x2 + 4 * x3 + x4) / 6,
				(y2 + 4 * y3 + y4) / 6);
		}

		public static Curve4Points UbSplineToBezier(Curve4Points cp)
		{
			return UbSplineToBezier(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
		}

		public static Curve4Points HermiteToBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			// Trans. matrix Hermite to Bezier
			//
			//  1       0       0       0
			//  1       0       1/3     0
			//  0       1       0       -1/3
			//  0       1       0       0
			//
			return new Curve4Points(
				x1,
				y1,
				(3 * x1 + x3) / 3,
				(3 * y1 + y3) / 3,
				(3 * x2 - x4) / 3,
				(3 * y2 - y4) / 3,
				x2,
				y2);
		}

		public static Curve4Points HermiteToBezier(Curve4Points cp)
		{
			return HermiteToBezier(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
		}
	}
}
