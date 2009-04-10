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
	using Agg.Xna.Paths;

	/// <summary>
	/// The Curve4Inc class.
	/// </summary>
	public class Curve4Incremental : ICurveApproximator
	{
		int m_num_steps;
		int m_step;
		double m_scale;
		double m_start_x;
		double m_start_y;
		double m_end_x;
		double m_end_y;
		double m_fx;
		double m_fy;
		double m_dfx;
		double m_dfy;
		double m_ddfx;
		double m_ddfy;
		double m_dddfx;
		double m_dddfy;
		double m_saved_fx;
		double m_saved_fy;
		double m_saved_dfx;
		double m_saved_dfy;
		double m_saved_ddfx;
		double m_saved_ddfy;

		public Curve4Incremental()
		{
			m_scale = 1.0;
		}

		public Curve4Incremental(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
			: this()
		{
			Initialize(x1, y1, x2, y2, x3, y3, x4, y4);
		}

		public Curve4Incremental(Curve4Points cp)
			: this(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7])
		{
		}

		public CurveApproximationMethod ApproximationMethod
		{
			get { return CurveApproximationMethod.Incremental; }
		}

		public double ApproximationScale
		{
			get { return m_scale; }
			set { m_scale = value; }
		}

		public double AngleTolerance
		{
			get { return 0.0; }
			set { }
		}

		public double CuspLimit
		{
			get { return 0.0; }
			set { }
		}

		public void Initialize(params double[] controlPoints)
		{
#if DEBUG
			if (controlPoints.Length != 8)
			{
				throw new ArgumentException("Cubic curves require 8 control points.", "controlPoints");
			}
#endif
			double x1 = controlPoints[0];
			double y1 = controlPoints[1];
			double x2 = controlPoints[2];
			double y2 = controlPoints[3];
			double x3 = controlPoints[4];
			double y3 = controlPoints[5];
			double x4 = controlPoints[6];
			double y4 = controlPoints[7];

			m_start_x = x1;
			m_start_y = y1;
			m_end_x = x4;
			m_end_y = y4;

			double dx1 = x2 - x1;
			double dy1 = y2 - y1;
			double dx2 = x3 - x2;
			double dy2 = y3 - y2;
			double dx3 = x4 - x3;
			double dy3 = y4 - y3;

			double len = (
				Math.Sqrt(dx1 * dx1 + dy1 * dy1) +
				Math.Sqrt(dx2 * dx2 + dy2 * dy2) +
				Math.Sqrt(dx3 * dx3 + dy3 * dy3)) * 0.25 * m_scale;

			m_num_steps = (int)len;

			if (m_num_steps < 4)
			{
				m_num_steps = 4;
			}

			double subdivide_step = 1.0 / m_num_steps;
			double subdivide_step2 = subdivide_step * subdivide_step;
			double subdivide_step3 = subdivide_step * subdivide_step * subdivide_step;

			double pre1 = 3.0 * subdivide_step;
			double pre2 = 3.0 * subdivide_step2;
			double pre4 = 6.0 * subdivide_step2;
			double pre5 = 6.0 * subdivide_step3;

			double tmp1x = x1 - x2 * 2.0 + x3;
			double tmp1y = y1 - y2 * 2.0 + y3;

			double tmp2x = (x2 - x3) * 3.0 - x1 + x4;
			double tmp2y = (y2 - y3) * 3.0 - y1 + y4;

			m_saved_fx = m_fx = x1;
			m_saved_fy = m_fy = y1;

			m_saved_dfx = m_dfx = (x2 - x1) * pre1 + tmp1x * pre2 + tmp2x * subdivide_step3;
			m_saved_dfy = m_dfy = (y2 - y1) * pre1 + tmp1y * pre2 + tmp2y * subdivide_step3;

			m_saved_ddfx = m_ddfx = tmp1x * pre4 + tmp2x * pre5;
			m_saved_ddfy = m_ddfy = tmp1y * pre4 + tmp2y * pre5;

			m_dddfx = tmp2x * pre5;
			m_dddfy = tmp2y * pre5;

			m_step = m_num_steps;
		}

		public void Initialize(Curve4Points cp)
		{
			Initialize(cp[0], cp[1], cp[2], cp[3], cp[4], cp[5], cp[6], cp[7]);
		}

		public void Reset()
		{
			m_num_steps = 0;
			m_step = -1;
		}

		public void Rewind(uint pathId)
		{
			if (m_num_steps == 0)
			{
				m_step = -1;
				return;
			}

			m_step = m_num_steps;
			m_fx = m_saved_fx;
			m_fy = m_saved_fy;
			m_dfx = m_saved_dfx;
			m_dfy = m_saved_dfy;
			m_ddfx = m_saved_ddfx;
			m_ddfy = m_saved_ddfy;
		}

		public PathCommand GetVertex(out double x, out double y)
		{
			if (m_step < 0)
			{
				x = y = 0;
				return PathCommand.Stop;
			}

			if (m_step == m_num_steps)
			{
				x = m_start_x;
				y = m_start_y;
				--m_step;
				return PathCommand.MoveTo;
			}
			else if (m_step == 0)
			{
				x = m_end_x;
				y = m_end_y;
				--m_step;
				return PathCommand.LineTo;
			}

			m_fx += m_dfx;
			m_fy += m_dfy;
			m_dfx += m_ddfx;
			m_dfy += m_ddfy;
			m_ddfx += m_dddfx;
			m_ddfy += m_dddfy;

			x = m_fx;
			y = m_fy;
			--m_step;
			return PathCommand.LineTo;
		}
	}
}
