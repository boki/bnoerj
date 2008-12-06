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
	/// Describes the behavior of the resolver when a dependency is missing.
	/// </summary>
	public enum DependencyMissingBehavior
	{
		/// <summary>
		/// Throw an exception if the dependency is missing.
		/// </summary>
		Throw,

		/// <summary>
		/// Set the target to null if the dependency is missing.
		/// </summary>
		Null
	}
}
