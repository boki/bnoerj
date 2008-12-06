// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Bnoerj.Sinject.Tests
{
	using System;
	using System.Collections.Generic;
	using Xunit;

	public interface IMock
	{
	}

	public class Dependency
	{
	}

	public class Concrete : IMock
	{
		[Inject]
		public Dependency Dependency { get; set; }
	}

	public class Concrete2
	{
		public Dependency Dependency { get; private set; }

		[Inject]
		public Concrete2(Dependency dependency)
		{
			Dependency = dependency;
		}
	}

	public class Concrete3
	{
		public Dependency Dependency1 { get; private set; }
		public Dependency Dependency2 { get; private set; }

		public Concrete3(Dependency dependency1, Dependency dependency2)
		{
			Dependency1 = dependency1;
			Dependency2 = dependency2;
		}
	}
}
