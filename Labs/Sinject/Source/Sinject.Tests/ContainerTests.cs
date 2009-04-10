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

	/// <summary>
	/// Provides tests for the Sinject class.
	/// </summary>
	public class ContainerTests
	{
		[Fact]
		public void Register_SingletonBehavior_ResolveShouldCreateOneInstanceOnly()
		{
			using (Container container = new Container())
			{
				container.Register<IMock, Concrete>();

				var concreteInstance1 = container.Resolve<IMock>();
				var concreteInstance2 = container.Resolve<IMock>();
				Assert.Same(concreteInstance1, concreteInstance2);
			}
		}

		[Fact]
		public void Register_PrecreatedSingleton_ResolveShouldReturnTheSameInstance()
		{
			using (Container container = new Container())
			{
				Concrete singleton = new Concrete();
				container.Register<IMock>(singleton);

				var concreteInstance1 = container.Resolve<IMock>();
				Assert.Same(concreteInstance1, singleton);
			}
		}

		[Fact]
		public void Register_TransientBehavior_ResolveShouldCreateOneInstancePerCall()
		{
			using (var container = new Container())
			{
				container.Register<IMock, Concrete>(Behavior.Transient);

				var concreteInstance1 = container.Resolve<IMock>();
				var concreteInstance2 = container.Resolve<IMock>();
				Assert.NotSame(concreteInstance1, concreteInstance2);
			}
		}

		[Fact]
		public void Resolve_DependencyProperty_ShouldBeInjected()
		{
			using (var container = new Container())
			{
				container.
					Register<Dependency>().
					Register<IMock, Concrete>(Behavior.Transient);

				var concreteInstance1 = container.Resolve<IMock>();
				var concrete = concreteInstance1 as Concrete;
				Assert.NotNull(concrete.Dependency);
			}
		}

		[Fact]
		public void Resolve_ConstructorDependency_ShouldBeInjected()
		{
			using (var container = new Container())
			{
				container.
					Register<Dependency>().
					Register<Concrete2>();

				var concreteInstance = container.Resolve<Concrete2>();
				Assert.NotNull(concreteInstance.Dependency);
			}
		}

		[Fact]
		public void Resolve_ProvidedConstructorDependencies_ShouldBeInjected()
		{
			using (var container = new Container())
			{
				Dependency dependency1 = new Dependency();
				Dependency dependency2 = new Dependency();
				container.
					Register<Concrete3>();

				var concreteInstance = container.Resolve<Concrete3>(dependency1, dependency2);
				Assert.Same(concreteInstance.Dependency1, dependency1);
				Assert.Same(concreteInstance.Dependency2, dependency2);
			}
		}
	}
}
