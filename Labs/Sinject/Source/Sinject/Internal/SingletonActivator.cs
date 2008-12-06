// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Bnoerj.Sinject.Internal
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Provides a singleton activator.
	/// </summary>
	internal class SingletonActivator : ActivatorBase, IDisposable
	{
		/// <summary>
		/// A value indicating whether the object has been disposed.
		/// </summary>
		bool disposed;

		/// <summary>
		/// Reference to the created instance.
		/// </summary>
		Object instance;

		/// <summary>
		/// Initializes a new instance of the SingletonActivator class using
		/// the specified concrete type and singleton instance.
		/// </summary>
		/// <param name="concreteType">Concrete type to construct.</param>
		/// <param name="instance">An instance to use.</param>
		public SingletonActivator(Type concreteType, Object instance)
			: base(concreteType)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Gets an instance of the type.
		/// </summary>
		/// <param name="sinject">The resolver to use to inject dependencies.</param>
		/// <param name="parameters">Constructor parameters.</param>
		/// <returns>An instance of the type.</returns>
		public override Object GetInstance(Sinject sinject, params Object[] parameters)
		{
			if (instance == null)
			{
				instance = ConstructInstance(sinject, parameters);
				FieldInfo[] injectionFields = GetInjectionFields();
				PropertyInfo[] injectionProperties = GetInjectionProperties();
				InjectDependencies(sinject, instance, injectionFields, injectionProperties);
			}

			return instance;
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
					IDisposable disposable = instance as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}

					instance = null;
					disposed = true;
				}
			}
		}
	}
}
