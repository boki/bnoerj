// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Bnoerj.Sinject
{
	using System;
	using System.Threading;

	/// <summary>
	/// Static wrapper for the Sinject container.
	/// </summary>
	public static class SinjectStatic
	{
		/// <summary>
		/// The reader-writer lock.
		/// </summary>
		static readonly ReaderWriterLockSlim syncRoot = new ReaderWriterLockSlim();

		/// <summary>
		/// The container instance.
		/// </summary>
		static readonly Sinject container = new Sinject();

		/// <summary>
		/// Resolves a registered type and returns an instance of it (based on
		/// the registred Lifetime policy).
		/// </summary>
		/// <typeparam name="T">Registered type.</typeparam>
		/// <returns>Instance of the registred type.</returns>
		public static T Resolve<T>()
			where T : class
		{
			syncRoot.EnterReadLock();
			try
			{
				return container.Resolve<T>();
			}
			finally
			{
				syncRoot.ExitReadLock();
			}
		}

		/// <summary>
		/// Resolves a registered type and returns an instance of it (based on
		/// the registred Lifetime policy).
		/// </summary>
		/// <typeparam name="T">Registered type.</typeparam>
		/// <param name="parameters">Constructor parameters.</param>
		/// <returns>Instance of the registred type.</returns>
		public static T Resolve<T>(params Object[] parameters)
			where T : class
		{
			syncRoot.EnterReadLock();
			try
			{
				return container.Resolve<T>(parameters);
			}
			finally
			{
				syncRoot.ExitReadLock();
			}
		}

		/// <summary>
		/// Registers a concrete type mapped to an interface in the container.
		/// The default Lifetime policy is Singleton.
		/// </summary>
		/// <typeparam name="TInterface">Interface type.</typeparam>
		/// <typeparam name="TConcrete">Concrete type.</typeparam>
		/// <returns>This - in order to enable fluent registering.</returns>
		public static Sinject Register<TInterface, TConcrete>()
		{
			syncRoot.EnterWriteLock();
			try
			{
				return container.Register<TInterface, TConcrete>(Behavior.Singleton);
			}
			finally
			{
				syncRoot.ExitWriteLock();
			}
		}

		/// <summary>
		/// Registers a concrete type mapped to an interface in the container.
		/// </summary>
		/// <typeparam name="TInterface">Interface type.</typeparam>
		/// <typeparam name="TConcrete">Concrete type.</typeparam>
		/// <param name="lifetime">Lifetime policy.</param>
		/// <returns>This - in order to enable fluent registering.</returns>
		public static Sinject Register<TInterface, TConcrete>(Behavior lifetime)
		{
			syncRoot.EnterWriteLock();
			try
			{
				return container.Register<TInterface, TConcrete>(lifetime);
			}
			finally
			{
				syncRoot.ExitWriteLock();
			}
		}

		/// <summary>
		/// Registers a concrete type in the container (not mapped to a specific
		/// interface).
		/// The default Lifetime policy is Singleton.
		/// </summary>
		/// <typeparam name="TConcrete">Concrete type.</typeparam>
		/// <returns>This - in order to enable fluent registering.</returns>
		public static Sinject Register<TConcrete>()
		{
			syncRoot.EnterWriteLock();
			try
			{
				return container.Register<TConcrete>();
			}
			finally
			{
				syncRoot.ExitWriteLock();
			}
		}

		/// <summary>
		/// Registers a concrete instance mapped to an interface in the container.
		/// </summary>
		/// <typeparam name="TInterface">Interface type.</typeparam>
		/// <param name="singleton">Singleton instance to register.</param>
		/// <returns>This - in order to enable fluent registering.</returns>
		public static Sinject Register<TInterface>(Object singleton)
		{
			syncRoot.EnterWriteLock();
			try
			{
				return container.Register<TInterface>(singleton);
			}
			finally
			{
				syncRoot.ExitWriteLock();
			}
		}

		/// <summary>
		/// Registers a concrete instance in the container (not mapped to a specific interface).
		/// </summary>
		/// <param name="singleton">Singleton instance to register.</param>
		/// <returns>This - in order to enable fluent registering.</returns>
		public static Sinject Register(Object singleton)
		{
			syncRoot.EnterWriteLock();
			try
			{
				return container.Register(singleton);
			}
			finally
			{
				syncRoot.ExitWriteLock();
			}
		}

		/// <summary>
		/// Empties the container (both the type map and registered singletons).
		/// If the registered singleton instances implements <c>System.IDisposable</c> the
		/// instances will be disposed before the singleton list is cleared.
		/// </summary>
		public static void Clear()
		{
			syncRoot.EnterWriteLock();
			try
			{
				container.Clear();
			}
			finally
			{
				syncRoot.ExitWriteLock();
			}
		}
	}
}
