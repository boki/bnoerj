// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Agg.Xna.Curves
{
	using System;
	using System.Collections.Generic;
	using Agg.Xna.Paths;

	public class Curve3Divisional : ICurveApproximator
	{
		const double DistanceEpsilon = 1e-30;
		const double CollinearityEpsilon = 1e-30;
		const double AngleToleranceEpsilon = 0.01;
		const int RecursionLimit = 32;

		double approximationScale;
		double distanceToleranceSquare;
		double angleTolerance;
		int count;
		List<PointD> points;

		public Curve3Divisional()
		{
			approximationScale = 1.0;
			points = new List<PointD>();
		}

		public Curve3Divisional(double x1, double y1, double x2, double y2, double x3, double y3)
			: this()
		{
			Initialize(x1, y1, x2, y2, x3, y3);
		}

		public CurveApproximationMethod ApproximationMethod
		{
			get { return CurveApproximationMethod.Divisional; }
		}

		public double ApproximationScale
		{
			get { return approximationScale; }
			set { approximationScale = value; }
		}

		public double AngleTolerance
		{
			get { return angleTolerance; }
			set { angleTolerance = value; }
		}

		public double CuspLimit
		{
			get { return 0.0; }
			set { }
		}

		public void Initialize(params double[] controlPoints)
		{
#if DEBUG
			if (controlPoints.Length != 6)
			{
				throw new ArgumentException("Quadratic curves require 6 control points.", "controlPoints");
			}
#endif
			double x1 = controlPoints[0];
			double y1 = controlPoints[1];
			double x2 = controlPoints[2];
			double y2 = controlPoints[3];
			double x3 = controlPoints[4];
			double y3 = controlPoints[5];

			points.Clear();
			distanceToleranceSquare = 0.5 / approximationScale;
			distanceToleranceSquare *= distanceToleranceSquare;
			Bezier(x1, y1, x2, y2, x3, y3);
			count = 0;
		}

		public void Reset()
		{
			points.Clear();
			count = 0;
		}

		public void Rewind(uint pathId)
        {
            count = 0;
        }

		public PathCommand GetVertex(out double x, out double y)
		{
			if (count >= points.Count)
			{
				x = 0;
				y = 0;
				return PathCommand.Stop;
			}

			PointD p = points[count++];
			x = p.X;
			y = p.Y;
			return count == 1 ? PathCommand.MoveTo : PathCommand.LineTo;
		}

		void Bezier(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			points.Add(new PointD(x1, y1));
			RecursiveBezier(x1, y1, x2, y2, x3, y3, 0);
			points.Add(new PointD(x3, y3));
		}

		void RecursiveBezier(double x1, double y1, double x2, double y2, double x3, double y3, int level)
		{
			if (level > RecursionLimit)
			{
				return;
			}

			//
			// Calculate all the mid-points of the line segments
			//

			double x12 = (x1 + x2) / 2;
			double y12 = (y1 + y2) / 2;
			double x23 = (x2 + x3) / 2;
			double y23 = (y2 + y3) / 2;
			double x123 = (x12 + x23) / 2;
			double y123 = (y12 + y23) / 2;

			double dx = x3 - x1;
			double dy = y3 - y1;
			double d = Math.Abs(((x2 - x3) * dy - (y2 - y3) * dx));
			double da;

			if (d > CollinearityEpsilon)
			{
				//
				// Regular case
				//

				if (d * d <= distanceToleranceSquare * (dx * dx + dy * dy))
				{
					// If the curvature doesn't exceed the distance_tolerance value
					// we tend to finish subdivisions.
					if (angleTolerance < AngleToleranceEpsilon)
					{
						points.Add(new PointD(x123, y123));
						return;
					}

					//
					// Angle & Cusp Condition
					//

					da = Math.Abs(Math.Atan2(y3 - y2, x3 - x2) - Math.Atan2(y2 - y1, x2 - x1));
					if (da >= Math.PI)
					{
						da = 2 * Math.PI - da;
					}

					if (da < angleTolerance)
					{
						// Finally we can stop the recursion
						points.Add(new PointD(x123, y123));
						return;
					}
				}
			}
			else
			{
				//
				// Collinear case
				//

				da = dx * dx + dy * dy;
				if (da == 0)
				{
					d = MathHelper.DistanceSquared(x1, y1, x2, y2);
				}
				else
				{
					d = ((x2 - x1) * dx + (y2 - y1) * dy) / da;
					if (d > 0 && d < 1)
					{
						// Simple collinear case, 1---2---3
						// We can leave just two endpoints
						return;
					}

					if (d <= 0)
					{
						d = MathHelper.DistanceSquared(x2, y2, x1, y1);
					}
					else if (d >= 1)
					{
						d = MathHelper.DistanceSquared(x2, y2, x3, y3);
					}
					else
					{
						d = MathHelper.DistanceSquared(x2, y2, x1 + d * dx, y1 + d * dy);
					}
				}

				if (d < distanceToleranceSquare)
				{
					points.Add(new PointD(x2, y2));
					return;
				}
			}

			// Continue subdivision
			RecursiveBezier(x1, y1, x12, y12, x123, y123, level + 1);
			RecursiveBezier(x123, y123, x23, y23, x3, y3, level + 1);
		}
	}
}
