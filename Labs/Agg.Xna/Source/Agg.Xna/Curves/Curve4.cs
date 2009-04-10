// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Agg.Xna.Curves
{
	using Agg.Xna.Paths;

	/// <summary>
	/// The Curve4 class.
	/// </summary>
	public class Curve4
	{
		ICurveApproximator approximator;
		Curve4Incremental curveInc;
		Curve4Divisional curveDiv;
		CurveApproximationMethod approximationMethod;

		public Curve4()
		{
			curveInc = new Curve4Incremental();
			curveDiv = new Curve4Divisional();
			approximationMethod = CurveApproximationMethod.Divisional;
			approximator = curveDiv;
		}

		public Curve4(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
			: this()
		{
			Initialize(x1, y1, x2, y2, x3, y3, x4, y4);
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

		public void Initialize(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			approximator.Initialize(x1, y1, x2, y2, x3, y3, x4, y4);
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
