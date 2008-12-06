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
	/// Describes the activation behavior of the types bound in the Sinject
	/// container.
	/// </summary>
	public enum Behavior
	{
		/// <summary>
		/// A new instance of the type will be created each time one is
		/// requested.
		/// </summary>
		Transient,

		/// <summary>
		/// Only a single instance of the type will be created and the same
		/// instance will be returned for each subsequent request.
		/// </summary>
		Singleton,

		/// <summary>
		/// One instance of the type will be created per thread.
		/// </summary>
		/// <remarks>Not implemented yet.</remarks>
		OnePerThread,
	}
}
