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

	/// <summary>
	/// Provides methods and properties for affine transformations.
	/// </summary>
	/// <remarks>
	/// Affine transformation are linear transformations in Cartesian coordinates
	/// (strictly speaking not only in Cartesian, but for the beginning we will 
	/// think so). They are rotation, scaling, translation and skewing.  
	/// After any affine transformation a line segment remains a line segment 
	/// and it will never become a curve.
	/// 
	/// There will be no math about matrix calculations, since it has been 
	/// described many times. Ask yourself a very simple question:
	/// "why do we need to understand and use some matrix stuff instead of just 
	/// rotating, scaling and so on". The answers are:
	///
	/// 1. Any combination of transformations can be done by only 4 multiplications
	///    and 4 additions in floating point.
	/// 2. One matrix transformation is equivalent to the number of consecutive
	///    discrete transformations, i.e. the matrix "accumulates" all transformations 
	///    in the order of their settings. Suppose we have 4 transformations: 
	///       * rotate by 30 degrees,
	///       * scale X to 2.0, 
	///       * scale Y to 1.5, 
	///       * move to (100, 100). 
	///    The result will depend on the order of these transformations, 
	///    and the advantage of matrix is that the sequence of discret calls:
	///    rotate(30), scaleX(2.0), scaleY(1.5), move(100,100) 
	///    will have exactly the same result as the following matrix transformations:
	///   
	///    affine_matrix m;
	///    m *= rotate_matrix(30); 
	///    m *= scaleX_matrix(2.0);
	///    m *= scaleY_matrix(1.5);
	///    m *= move_matrix(100,100);
	///
	///    m.transform_my_point_at_last(x, y);
	///
	/// What is the good of it? In real life we will set-up the matrix only once
	/// and then transform many points, let alone the convenience to set any 
	/// combination of transformations.
	/// </remarks>
	/// <example>
	/// So, how to use it? Very easy - literally as it's shown above. Not quite,
	/// let us write a correct example:
	///
	/// Agg.Lite.AffineTransform m;
	/// m *= Agg.Lite.AffineTransform.Rotation(30.0 * 3.1415926 / 180.0);
	/// m *= Agg.Lite.AffineTransform.Scaling(2.0, 1.5);
	/// m *= Agg.Lite.AffineTransform.Translation(100.0, 100.0);
	/// m.transform(&x, &y);
	///
	/// The affine matrix is all you need to perform any linear transformation,
	/// but all transformations have origin point (0,0). It means that we need to 
	/// use 2 translations if we want to rotate someting around (100,100):
	/// 
	/// m *= Agg.Lite.AffineTransform.Translation(-100.0, -100.0);         // move to (0,0)
	/// m *= Agg.Lite.AffineTransform.Rotation(30.0 * 3.1415926 / 180.0);  // rotate
	/// m *= Agg.Lite.AffineTransform.Translation(100.0, 100.0);           // move back to (100,100)
	/// </example>
	public struct AffineTransform
	{
		const double Epsilon = 1e-14;

		/// <summary>
		/// Returns an instance of the identity matrix.
		/// </summary>
		public static AffineTransform Identity = new AffineTransform(1, 0, 0, 1, 0, 0);

		/// <summary>
		/// The scale along the x-axis.
		/// </summary>
		public double sx;

		/// <summary>
		/// The shear value along the y-axis.
		/// </summary>
		public double shy;

		/// <summary>
		/// The shear value along the x-axis.
		/// </summary>
		public double shx;

		/// <summary>
		/// The scale value along the y-axis.
		/// </summary>
		public double sy;

		/// <summary>
		/// The translation value along the x-axis.
		/// </summary>
		public double tx;

		/// <summary>
		/// The translation value along the y-axis.
		/// </summary>
		public double ty;

		public AffineTransform(double sx, double shy, double shx, double sy, double tx, double ty)
		{
			this.sx = sx;
			this.shy = shy;
			this.shx = shy;
			this.sy = sy;
			this.tx = tx;
			this.ty = ty;
		}

		public AffineTransform(ref AffineTransform other)
		{
			this.sx = other.sx;
			this.shy = other.shy;
			this.shx = other.shy;
			this.sy = other.sy;
			this.tx = other.tx;
			this.ty = other.ty;
		}

		public static AffineTransform CreateTranslation(double x, double y)
		{
			return new AffineTransform(1, 0, 0, 1, x, y);
		}

		public static AffineTransform CreateScale(double sx, double sy)
		{
			return new AffineTransform(sx, 0, 0, sy, 0, 0);
		}

		public static AffineTransform CreateRotation(double a)
		{
			return new AffineTransform(Math.Cos(a), Math.Sin(a), -Math.Sin(a), Math.Cos(a), 0.0, 0.0);
		}

		public static AffineTransform operator *(AffineTransform left, AffineTransform right)
		{
			return left.Multiply(ref right);
		}

		public static AffineTransform operator /(AffineTransform left, AffineTransform right)
		{
			return left.MultiplyInverse(ref right);
		}

		public static bool operator ==(AffineTransform left, AffineTransform right)
		{
			return left.IsEqual(ref right);
		}

		public static bool operator !=(AffineTransform left, AffineTransform right)
		{
			return left.IsEqual(ref right) == false;
		}

		public AffineTransform Translate(double x, double y)
		{
			tx += x;
			ty += y;

			return this;
		}

		public AffineTransform Rotate(double a)
		{
			double ca = Math.Cos(a);
			double sa = Math.Sin(a);
			double t0 = sx * ca - shy * sa;
			double t2 = shx * ca - sy * sa;
			double t4 = tx * ca - ty * sa;
			shy = sx * sa + shy * ca;
			sy = shx * sa + sy * ca;
			ty = tx * sa + ty * ca;
			sx = t0;
			shx = t2;
			tx = t4;

			return this;
		}

		public AffineTransform Scale(double s)
		{
			double m = s;
			sx *= m;
			shx *= m;
			tx *= m;
			shy *= m;
			sy *= m;
			ty *= m;

			return this;
		}

		public AffineTransform Scale(double x, double y)
		{
			double mm0 = x;
			double mm3 = y;
			sx *= mm0;
			shx *= mm0;
			tx *= mm0;
			shy *= mm3;
			sy *= mm3;
			ty *= mm3;

			return this;
		}

		// Multiply matrix to another one
		public AffineTransform Multiply(ref AffineTransform m)
		{
			double t0 = sx * m.sx + shy * m.shx;
			double t2 = shx * m.sx + sy * m.shx;
			double t4 = tx * m.sx + ty * m.shx + m.tx;
			shy = sx * m.shy + shy * m.sy;
			sy = shx * m.shy + sy * m.sy;
			ty = tx * m.shy + ty * m.sy + m.ty;
			sx = t0;
			shx = t2;
			tx = t4;
			return this;
		}

		// Multiply "m" to "this" and assign the result to "this"
		public AffineTransform Premultiply(ref AffineTransform m)
		{
			AffineTransform t = m;
			this = t.Multiply(ref this);

			return this;
		}

		// Multiply matrix to inverse of another one
		public AffineTransform MultiplyInverse(ref AffineTransform m)
		{
			AffineTransform t = m;
			t.Invert();
			return Multiply(ref t);
		}

		// Multiply inverse of "m" to "this" and assign the result to "this"
		public AffineTransform PremultiplyInverse(ref AffineTransform m)
		{
			AffineTransform t = m;
			t.Invert();
			this = t.Multiply(ref this);
			return this;
		}

		// Invert matrix. Do not try to invert degenerate matrices, 
		// there's no check for validity. If you set scale to 0 and 
		// then try to invert matrix, expect unpredictable result.
		public AffineTransform Invert()
		{
			double d = DeterminantReciprocal();

			double t0 = sy * d;
			sy = sx * d;
			shy = -shy * d;
			shx = -shx * d;

			double t4 = -tx * t0 - ty * shx;
			ty = -tx * shy - ty * sy;

			sx = t0;
			tx = t4;

			return this;
		}

		// Mirroring around X
		public AffineTransform FlipX()
		{
			sx = -sx;
			shy = -shy;
			tx = -tx;

			return this;
		}

		// Mirroring around Y
		public AffineTransform FlipY()
		{
			shx = -shx;
			sy = -sy;
			ty = -ty;

			return this;
		}

		// Direct transformation of x and y
		public void Transform(ref double x, ref double y)
		{
			double ox = x;
			x = ox * sx + y * shx + tx;
			y = ox * shy + y * sy + ty;
		}

		public void Transform(ref PointD point)
		{
			double ox = point.X;
			point.X = ox * sx + point.Y * shx + tx;
			point.Y = ox * shy + point.Y * sy + ty;
		}

		// Direct transformation of x and y, 2x2 matrix only, no translation
		public void Transform2x2(ref double x, ref double y)
		{
			double ox = x;
			x = ox * sx + y * shx;
			y = ox * shy + y * sy;
		}

		// Inverse transformation of x and y. It works slower than the 
		// direct transformation. For massive operations it's better to 
		// invert() the matrix and then use direct transformations. 
		public void InverseTransform(ref double x, ref double y)
		{
			double d = DeterminantReciprocal();
			double a = (x - tx) * d;
			double b = (y - ty) * d;
			x = a * sy - b * shx;
			y = b * sx - a * shy;
		}

		// Calculate the determinant of matrix
		public double Determinant()
		{
			return sx * sy - shy * shx;
		}

		// Calculate the reciprocal of the determinant
		public double DeterminantReciprocal()
		{
			return 1.0 / (sx * sy - shy * shx);
		}

		// Get the average scale (by X and Y). 
		// Basically used to calculate the approximation_scale when
		// decomposinting curves into line segments.
		public double Scale()
		{
			double x = 0.707106781 * sx + 0.707106781 * shx;
			double y = 0.707106781 * shy + 0.707106781 * sy;
			return Math.Sqrt(x * x + y * y);
		}

		// Check to see if the matrix is not degenerate
		public bool IsValid()
		{
			return IsValid(Epsilon);
		}
		public bool IsValid(double epsilon)
		{
			return Math.Abs(sx) > epsilon && Math.Abs(sy) > epsilon;
		}

		// Check to see if it's an identity matrix
		public bool IsIdentity()
		{
			return IsIdentity(Epsilon);
		}
		public bool IsIdentity(double epsilon)
		{
			return MathHelper.IsEqual(sx, 1.0, epsilon) &&
				   MathHelper.IsEqual(shy, 0.0, epsilon) &&
				   MathHelper.IsEqual(shx, 0.0, epsilon) &&
				   MathHelper.IsEqual(sy, 1.0, epsilon) &&
				   MathHelper.IsEqual(tx, 0.0, epsilon) &&
				   MathHelper.IsEqual(ty, 0.0, epsilon);
		}

		// Check to see if two matrices are equal
		public bool IsEqual(ref AffineTransform m)
		{
			return IsEqual(ref m, Epsilon);
		}
		public bool IsEqual(ref AffineTransform m, double epsilon)
		{
			return MathHelper.IsEqual(sx, m.sx, epsilon) &&
				   MathHelper.IsEqual(shy, m.shy, epsilon) &&
				   MathHelper.IsEqual(shx, m.shx, epsilon) &&
				   MathHelper.IsEqual(sy, m.sy, epsilon) &&
				   MathHelper.IsEqual(tx, m.tx, epsilon) &&
				   MathHelper.IsEqual(ty, m.ty, epsilon);
		}

		// Determine the major parameters. Use with caution considering 
		// possible degenerate cases.
		public double Rotation()
		{
			double x1 = 0.0;
			double y1 = 0.0;
			double x2 = 1.0;
			double y2 = 0.0;
			Transform(ref x1, ref y1);
			Transform(ref x2, ref y2);
			return Math.Atan2(y2 - y1, x2 - x1);
		}
		public void Translation(ref double dx, ref double dy)
		{
			dx = tx;
			dy = ty;
		}
		public void Scaling(ref double dx, ref double dy)
		{
			double x1 = 0.0;
			double y1 = 0.0;
			double x2 = 1.0;
			double y2 = 1.0;
			AffineTransform t = new AffineTransform(ref this);
			t *= AffineTransform.CreateRotation(-Rotation());
			t.Transform(ref x1, ref y1);
			t.Transform(ref x2, ref y2);
			dx = x2 - x1;
			dy = y2 - y1;
		}

		public void ScalingAbsolute(ref double dx, ref double dy)
		{
			// Used to calculate scaling coefficients in image resampling. 
			// When there is considerable shear this method gives us much
			// better estimation than just sx, sy.
			dx = Math.Sqrt(sx * sx + shx * shx);
			dy = Math.Sqrt(shy * shy + sy * sy);
		}

		public override bool Equals(object obj)
		{
			if (obj is AffineTransform)
			{
				return this == (AffineTransform)obj;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return sx.GetHashCode() + shy.GetHashCode() +
				shx.GetHashCode() + sy.GetHashCode() +
				tx.GetHashCode() + ty.GetHashCode();
		}
	}
}
