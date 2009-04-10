// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna.Curves
{
	using System;
	using System.Collections.Generic;
	using Agg.Xna.Paths;

	/// <summary>
	/// The Curve4Div class.
	/// </summary>
	public class Curve4Divisional : ICurveApproximator
	{
		const uint CurveRecursionLimit = 32;
		const double CurveDistanceEpsilon = 1e-30;
		const double CurveCollinearityEpsilon = 1e-30;
		const double CurveAngleToleranceEpsilon = 0.01;

		double m_approximation_scale;
		double m_distance_tolerance_square;
		double m_angle_tolerance;
		double m_cusp_limit;
		int m_count;
		List<PointD> m_points;

		public Curve4Divisional()
		{
			m_approximation_scale = 1.0;
		}

		public Curve4Divisional(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
			: this()
		{
			Initialize(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public Curve4Divisional(Curve4Points cp)
			: this(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7])
		{
		}

		public CurveApproximationMethod ApproximationMethod
		{
			get { return CurveApproximationMethod.Divisional; }
		}

		public double ApproximationScale
		{
			get { return m_approximation_scale; }
			set { m_approximation_scale = value; }
		}

		public double AngleTolerance
		{
			get { return m_angle_tolerance; }
			set { m_angle_tolerance = value; }
		}

		public double CuspLimit
		{
			get { return m_cusp_limit == 0.0 ? 0.0 : Math.PI - m_cusp_limit; }
			set { m_cusp_limit = value == 0.0 ? 0.0 : Math.PI - value; }
		}

		public void Initialize(params double[] controlPoints)
		{
#if DEBUG
			if (controlPoints.Length != 8)
			{
				throw new ArgumentException("Cubic curves require 8 control points.", "controlPoints");
			}
#endif
			if (m_points == null)
			{
				m_points = new List<PointD>();
			}

			m_points.Clear();
			m_distance_tolerance_square = 0.5 / m_approximation_scale;
			m_distance_tolerance_square *= m_distance_tolerance_square;

			Bezier(controlPoints[0], controlPoints[1],
				controlPoints[2], controlPoints[3],
				controlPoints[4], controlPoints[5],
				controlPoints[6], controlPoints[7]);

			m_count = 0;
		}

		public void Initialize(Curve4Points cp)
		{
			Initialize(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
		}

		public void Reset()
		{
			m_points.Clear();
			m_count = 0;
		}

		public void Rewind(uint pathId)
		{
			m_count = 0;
		}

		public PathCommand GetVertex(out double x, out double y)
		{
			if (m_count >= m_points.Count)
			{
				x = y = 0;
				return PathCommand.Stop;
			}

			PointD p = m_points[m_count++];
			x = p.X;
			y = p.Y;

			return m_count == 1 ? PathCommand.MoveTo : PathCommand.LineTo;
		}

		void Bezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			m_points.Add(new PointD(x1, y1));
			RecursiveBezier(x1, y1, x2, y2, x3, y3, x4, y4, 0);
			m_points.Add(new PointD(x4, y4));
		}

		void RecursiveBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, uint level)
		{
			if (level > CurveRecursionLimit)
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
			double x34 = (x3 + x4) / 2;
			double y34 = (y3 + y4) / 2;
			double x123 = (x12 + x23) / 2;
			double y123 = (y12 + y23) / 2;
			double x234 = (x23 + x34) / 2;
			double y234 = (y23 + y34) / 2;
			double x1234 = (x123 + x234) / 2;
			double y1234 = (y123 + y234) / 2;

			//
			// Try to approximate the full cubic curve by a single straight line
			//

			double dx = x4 - x1;
			double dy = y4 - y1;

			double d2 = Math.Abs(((x2 - x4) * dy - (y2 - y4) * dx));
			double d3 = Math.Abs(((x3 - x4) * dy - (y3 - y4) * dx));
			double da1, da2, k;

			int foo =
				(int)((d2 > CurveCollinearityEpsilon ? 1 : 0) << 1) +
				(int)(d3 > CurveCollinearityEpsilon ? 1 : 0);
			switch (foo)
			{
			case 0:
				//
				// All collinear OR p1==p4
				//

				k = dx * dx + dy * dy;
				if (k == 0)
				{
					d2 = MathHelper.DistanceSquared(x1, y1, x2, y2);
					d3 = MathHelper.DistanceSquared(x4, y4, x3, y3);
				}
				else
				{
					k = 1 / k;
					da1 = x2 - x1;
					da2 = y2 - y1;
					d2 = k * (da1 * dx + da2 * dy);
					da1 = x3 - x1;
					da2 = y3 - y1;
					d3 = k * (da1 * dx + da2 * dy);
					if (d2 > 0 && d2 < 1 && d3 > 0 && d3 < 1)
					{
						// Simple collinear case, 1---2---3---4
						// We can leave just two endpoints
						return;
					}

					if (d2 <= 0)
					{
						d2 = MathHelper.DistanceSquared(x2, y2, x1, y1);
					}
					else if (d2 >= 1)
					{
						d2 = MathHelper.DistanceSquared(x2, y2, x4, y4);
					}
					else
					{
						d2 = MathHelper.DistanceSquared(x2, y2, x1 + d2 * dx, y1 + d2 * dy);
					}

					if (d3 <= 0)
					{
						d3 = MathHelper.DistanceSquared(x3, y3, x1, y1);
					}
					else if (d3 >= 1)
					{
						d3 = MathHelper.DistanceSquared(x3, y3, x4, y4);
					}
					else
					{
						d3 = MathHelper.DistanceSquared(x3, y3, x1 + d3 * dx, y1 + d3 * dy);
					}
				}

				if (d2 > d3)
				{
					if (d2 < m_distance_tolerance_square)
					{
						m_points.Add(new PointD(x2, y2));
						return;
					}
				}
				else
				{
					if (d3 < m_distance_tolerance_square)
					{
						m_points.Add(new PointD(x3, y3));
						return;
					}
				}
				break;

			case 1:
				//
				// p1,p2,p4 are collinear, p3 is significant
				//

				if (d3 * d3 <= m_distance_tolerance_square * (dx * dx + dy * dy))
				{
					if (m_angle_tolerance < CurveAngleToleranceEpsilon)
					{
						m_points.Add(new PointD(x23, y23));
						return;
					}

					//
					// Angle Condition
					//

					da1 = Math.Abs(Math.Atan2(y4 - y3, x4 - x3) - Math.Atan2(y3 - y2, x3 - x2));
					if (da1 >= Math.PI)
						da1 = 2 * Math.PI - da1;

					if (da1 < m_angle_tolerance)
					{
						m_points.Add(new PointD(x2, y2));
						m_points.Add(new PointD(x3, y3));
						return;
					}

					if (m_cusp_limit != 0.0)
					{
						if (da1 > m_cusp_limit)
						{
							m_points.Add(new PointD(x3, y3));
							return;
						}
					}
				}
				break;

			case 2:
				//
				// p1,p3,p4 are collinear, p2 is significant
				//

				if (d2 * d2 <= m_distance_tolerance_square * (dx * dx + dy * dy))
				{
					if (m_angle_tolerance < CurveAngleToleranceEpsilon)
					{
						m_points.Add(new PointD(x23, y23));
						return;
					}

					//
					// Angle Condition
					//
					da1 = Math.Abs(Math.Atan2(y3 - y2, x3 - x2) - Math.Atan2(y2 - y1, x2 - x1));
					if (da1 >= Math.PI)
					{
						da1 = 2 * Math.PI - da1;
					}

					if (da1 < m_angle_tolerance)
					{
						m_points.Add(new PointD(x2, y2));
						m_points.Add(new PointD(x3, y3));
						return;
					}

					if (m_cusp_limit != 0.0)
					{
						if (da1 > m_cusp_limit)
						{
							m_points.Add(new PointD(x2, y2));
							return;
						}
					}
				}
				break;

			case 3:
				//
				// Regular case
				//
				if ((d2 + d3) * (d2 + d3) <= m_distance_tolerance_square * (dx * dx + dy * dy))
				{
					// If the curvature doesn't exceed the distance_tolerance value
					// we tend to finish subdivisions.
					if (m_angle_tolerance < CurveAngleToleranceEpsilon)
					{
						m_points.Add(new PointD(x23, y23));
						return;
					}

					//
					// Angle & Cusp Condition
					//

					k = Math.Atan2(y3 - y2, x3 - x2);
					da1 = Math.Abs(k - Math.Atan2(y2 - y1, x2 - x1));
					da2 = Math.Abs(Math.Atan2(y4 - y3, x4 - x3) - k);
					if (da1 >= Math.PI)
					{
						da1 = 2 * Math.PI - da1;
					}

					if (da2 >= Math.PI)
					{
						da2 = 2 * Math.PI - da2;
					}

					if (da1 + da2 < m_angle_tolerance)
					{
						// Finally we can stop the recursion
						m_points.Add(new PointD(x23, y23));
						return;
					}

					if (m_cusp_limit != 0.0)
					{
						if (da1 > m_cusp_limit)
						{
							m_points.Add(new PointD(x2, y2));
							return;
						}

						if (da2 > m_cusp_limit)
						{
							m_points.Add(new PointD(x3, y3));
							return;
						}
					}
				}
				break;
			}

			// Continue subdivision
			RecursiveBezier(x1, y1, x12, y12, x123, y123, x1234, y1234, level + 1);
			RecursiveBezier(x1234, y1234, x234, y234, x34, y34, x4, y4, level + 1);
		}
	}
}
