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
	/// The ICurveApproximator interface.
	/// </summary>
	public interface ICurveApproximator
	{
		/// <summary>
		/// Gets the approximation method.
		/// </summary>
		CurveApproximationMethod ApproximationMethod { get; }

		/// <summary>
		/// Gets or sets the approximation scale.
		/// </summary>
		double ApproximationScale { get; set; }

		/// <summary>
		/// Gets or sets the angle of tolerance.
		/// </summary>
		double AngleTolerance { get; set; }

		/// <summary>
		/// Gets or sets the cusp limit.
		/// </summary>
		double CuspLimit { get; set; }

		/// <summary>
		/// Initializes the instance to the new values.
		/// </summary>
		/// <param name="controlPoints">The coordinates of the curves control points.</param>
		void Initialize(params double[] controlPoints);

		/// <summary>
		/// Resets the approximator.
		/// </summary>
		void Reset();

		/// <summary>
		/// Resinds the specified path.
		/// </summary>
		/// <param name="pathId">The path ID.</param>
		void Rewind(uint pathId);

		/// <summary>
		/// Gets the next vertex of the approximation.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		PathCommand GetVertex(out double x, out double y);
	}
}
