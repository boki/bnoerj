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
	using Agg.Xna.Paths;

	public class Curve3Incremental : ICurveApproximator
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
		double m_saved_fx;
		double m_saved_fy;
		double m_saved_dfx;
		double m_saved_dfy;

		public Curve3Incremental()
		{
			m_scale = 1.0;
		}

		public Curve3Incremental(double x1, double y1, double x2, double y2, double x3, double y3)
			: this()
		{
			m_scale = 1.0;
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

			m_start_x = x1;
			m_start_y = y1;
			m_end_x = x3;
			m_end_y = y3;

			double dx1 = x2 - x1;
			double dy1 = y2 - y1;
			double dx2 = x3 - x2;
			double dy2 = y3 - y2;

			double len = Math.Sqrt(dx1 * dx1 + dy1 * dy1) +
				Math.Sqrt(dx2 * dx2 + dy2 * dy2);

			m_num_steps = (int)(len * 0.25 * m_scale);

			if (m_num_steps < 4)
			{
				m_num_steps = 4;
			}

			double subdivide_step = 1.0 / m_num_steps;
			double subdivide_step2 = subdivide_step * subdivide_step;

			double tmpx = (x1 - x2 * 2.0 + x3) * subdivide_step2;
			double tmpy = (y1 - y2 * 2.0 + y3) * subdivide_step2;

			m_saved_fx = m_fx = x1;
			m_saved_fy = m_fy = y1;

			m_saved_dfx = m_dfx = tmpx + (x2 - x1) * (2.0 * subdivide_step);
			m_saved_dfy = m_dfy = tmpy + (y2 - y1) * (2.0 * subdivide_step);

			m_ddfx = tmpx * 2.0;
			m_ddfy = tmpy * 2.0;

			m_step = m_num_steps;
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
		}

		public PathCommand GetVertex(out double x, out double y)
		{
			if (m_step < 0)
			{
				x = 0;
				y = 0;
				return PathCommand.Stop;
			}

			if (m_step == m_num_steps)
			{
				x = m_start_x;
				y = m_start_y;
				--m_step;
				return PathCommand.MoveTo;
			}

			if (m_step == 0)
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
			x = m_fx;
			y = m_fy;
			--m_step;
			return PathCommand.LineTo;
		}
	}
}
