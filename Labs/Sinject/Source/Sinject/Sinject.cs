// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Bnoerj.Sinject
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Bnoerj.Sinject.Internal;

	/// <summary>
	/// A simple IoC/DI container designed to be small/simple and used with
	/// the XNA FX.
	/// </summary>
	public class Sinject : IDisposable
	{
		/// <summary>
		/// An empty object array.
		/// </summary>
		internal readonly Object[] EmptyObjectArray;

		/// <summary>
		/// Map of registered type to activators.
		/// </summary>
		Dictionary<Type, ActivatorBase> map;

		/// <summary>
		/// A value indicating whether the object has been disposed.
		/// </summary>
		bool disposed;

		/// <summary>
		/// Initializes a new instance of the Sinject class.
		/// </summary>
		public Sinject()
		{
			EmptyObjectArray = new Object[] { };
			map = new Dictionary<Type, ActivatorBase>();
		}

		/// <summary>
		/// Registers a concrete type mapped to an interface in the container
		/// using the default Singleton activation behavior.
		/// </summary>
		/// <typeparam name="TInterface">Interface type.</typeparam>
		/// <typeparam name="TConcrete">Concrete type.</typeparam>
		/// <returns>This - in order to enable fluent registering.</returns>
		public Sinject Register<TInterface, TConcrete>()
		{
			Register<TInterface, TConcrete>(Behavior.Singleton);

			return this;
		}

		/// <summary>
		/// Registers a concrete type mapped to an interface in the container.
		/// </summary>
		/// <typeparam name="TInterface">Interface type.</typeparam>
		/// <typeparam name="TConcrete">Concrete type.</typeparam>
		/// <param name="behavior">Activation behavior.</param>
		/// <returns>This - in order to enable fluent registering.</returns>
		public Sinject Register<TInterface, TConcrete>(Behavior behavior)
		{
			Register(typeof(TInterface), typeof(TConcrete), behavior, null);

			return this;
		}

		/// <summary>
		/// Registers a concrete type in the container (not mapped to a specific
		/// interface) using the default Singleton activation behavior.
		/// </summary>
		/// <typeparam name="TConcrete">Concrete type.</typeparam>
		/// <returns>This - in order to enable fluent registering.</returns>
		public Sinject Register<TConcrete>()
		{
			Register(typeof(TConcrete), typeof(TConcrete), Behavior.Singleton, null);

			return this;
		}

		/// <summary>
		/// Registers a concrete singleton instance mapped to an interface in
		/// the container.
		/// </summary>
		/// <typeparam name="TInterface">Interface type.</typeparam>
		/// <param name="singleton">Singleton instance to register.</param>
		/// <returns>This - in order to enable fluent registering.</returns>
		public Sinject Register<TInterface>(Object singleton)
		{
			Register(typeof(TInterface), singleton.GetType(), Behavior.Singleton, singleton);

			return this;
		}

		/// <summary>
		/// Registers a concrete singleton instance in the container (not mapped
		/// to a specific interface).
		/// </summary>
		/// <param name="singleton">Singleton instance to register.</param>
		/// <returns>This - in order to enable fluent registering.</returns>
		public Sinject Register(Object singleton)
		{
			Type type = singleton.GetType();
			Register(type, type, Behavior.Singleton, singleton);

			return this;
		}

		/// <summary>
		/// Resolves a registered type and returns an instance of it (based on
		/// the registered activation behavior).
		/// </summary>
		/// <typeparam name="T">Registered type.</typeparam>
		/// <returns>Instance of the registred type.</returns>
		public T Resolve<T>()
			where T : class
		{
			return Resolve(typeof(T), EmptyObjectArray) as T;
		}

		/// <summary>
		/// Resolves a registered type and returns an instance of it (based on
		/// the registered activation behavior).
		/// </summary>
		/// <typeparam name="T">Registered type.</typeparam>
		/// <param name="parameters">Constructor parameters.</param>
		/// <returns>Instance of the registred type.</returns>
		public T Resolve<T>(params Object[] parameters)
			where T : class
		{
			return Resolve(typeof(T), parameters) as T;
		}

		/// <summary>
		/// Empties the container. If the registered singleton instances
		/// implement <c>System.IDisposable</c> the instance will be disposed.
		/// </summary>
		public void Clear()
		{
			foreach (ActivatorBase activator in map.Values)
			{
				IDisposable disposable = activator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}

			map.Clear();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Retrieves an instance for the speficied type.
		/// </summary>
		/// <param name="type">Interface type.</param>
		/// <returns>Instance of the registered concrete type for the interface type.</returns>
		internal Object Resolve(Type type)
		{
			return Resolve(type, EmptyObjectArray);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">A value indicating whether the method is called from Dispose() or not.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposed == true)
			{
				if (disposing == true)
				{
					Clear();

					map = null;
					disposed = true;
				}
			}
		}

		/// <summary>
		/// Registers the specified concrete type to the interface type, using
		/// the behavior and singleton instance.
		/// </summary>
		/// <param name="interfaceType">Interface type.</param>
		/// <param name="concreteType">Concrete type.</param>
		/// <param name="behavior">Activation behavior.</param>
		/// <param name="instance">Singleton instance.</param>
		void Register(Type interfaceType, Type concreteType, Behavior behavior, Object instance)
		{
			ActivatorBase activator;

			if (behavior == Behavior.Transient)
			{
				activator = new TransientActivator(concreteType);
			}
			else
			{
				activator = new SingletonActivator(concreteType, instance);
			}

			map.Add(interfaceType, activator);
		}

		/// <summary>
		/// Retrieves an instance for the speficied type using the parameters
		/// to construct the instanve.
		/// </summary>
		/// <param name="type">Interface type.</param>
		/// <param name="parameters">Parameters to use during construction.</param>
		/// <returns>Instance of the registered concrete type for the interface type.</returns>
		Object Resolve(Type type, params Object[] parameters)
		{
			ActivatorBase activator;
			if (map.TryGetValue(type, out activator) == true)
			{
				return activator.GetInstance(this, parameters);
			}

			return null;
		}
	}
}
