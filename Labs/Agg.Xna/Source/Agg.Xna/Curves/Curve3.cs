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
	using System.Linq;
	using System.Text;
	using Agg.Xna.Paths;

	public class Curve3
	{
		ICurveApproximator approximator;
		Curve3Incremental curveInc;
		Curve3Divisional curveDiv;
		CurveApproximationMethod approximationMethod;

		public Curve3()
		{
			curveInc = new Curve3Incremental();
			curveDiv = new Curve3Divisional();
			approximationMethod = CurveApproximationMethod.Divisional;
			approximator = curveDiv;
		}

		public Curve3(double x1, double y1, double x2, double y2, double x3, double y3)
			: this()
		{
			Initialize(x1, y1, x2, y2, x3, y3);
		}

		public CurveApproximationMethod ApproximationMethod
		{
			get { return approximationMethod; }
			set
			{
				if (approximationMethod != value)
				{
					if (value == CurveApproximationMethod.Incremental)
					{
						approximator = curveInc;
					}
					else
					{
						approximator = curveDiv;
					}
				}

				approximationMethod = value;
			}
		}

		public double ApproximationScale
		{
			get { return approximator.ApproximationScale; }
			set { approximator.ApproximationScale = value; }
		}

		public double AngleTolerance
		{
			get { return approximator.AngleTolerance; }
			set { approximator.AngleTolerance = value; }
		}

		public double CuspLimit
		{
			get { return approximator.CuspLimit; }
			set { approximator.CuspLimit = value; }
		}

		public void Reset()
		{
			approximator.Reset();
		}

		public void Initialize(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			approximator.Initialize(x1, y1, x2, y2, x3, y3);
		}

		public void Rewind(uint path_id)
		{
			approximator.Rewind(path_id);
		}

		public PathCommand GetVertex(out double x, out double y)
		{
			return approximator.GetVertex(out x, out y);
		}
	}
}
