// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Bnoerj.Sinject
{
	using System;

	/// <summary>
	/// Marker attribute for indicating injection of dependencies.
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
	public sealed class InjectAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the InjectAttribute class.
		/// </summary>
		public InjectAttribute()
		{
			DependencyMissingBehavior = DependencyMissingBehavior.Null;
		}

		/// <summary>
		/// Initializes a new instance of the InjectAttribute class using the
		/// specified behavior.
		/// </summary>
		/// <param name="dependencyMissingBehavior">Behavior for missing dependencies.</param>
		public InjectAttribute(DependencyMissingBehavior dependencyMissingBehavior)
		{
			DependencyMissingBehavior = dependencyMissingBehavior;
		}

		/// <summary>
		/// Gets the behavior for missing dependencies.
		/// </summary>
		public DependencyMissingBehavior DependencyMissingBehavior
		{
			get;
			private set;
		}
	}
}
