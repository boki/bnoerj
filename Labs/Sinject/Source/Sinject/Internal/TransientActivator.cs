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
	/// Provides a transient activator.
	/// </summary>
	internal class TransientActivator : ActivatorBase
	{
		/// <summary>
		/// Cached field infos of fields marked with the InjectAttribute.
		/// </summary>
		FieldInfo[] injectionFields;

		/// <summary>
		/// Cached property infos of properties marked with the InjectAttribute.
		/// </summary>
		PropertyInfo[] injectionProperties;

		/// <summary>
		/// Initializes a new instance of the TransientActivator class using the
		/// specified concrete type.
		/// </summary>
		/// <param name="concreteType">Concrete type to construct.</param>
		public TransientActivator(Type concreteType)
			: base(concreteType)
		{
			injectionFields = GetInjectionFields();
			injectionProperties = GetInjectionProperties();
		}

		/// <summary>
		/// Gets an instance of the type.
		/// </summary>
		/// <param name="sinject">The resolver to use to inject dependencies.</param>
		/// <param name="parameters">Constructor parameters.</param>
		/// <returns>An instance of the type.</returns>
		public override Object GetInstance(Sinject sinject, params Object[] parameters)
		{
			Object instance = ConstructInstance(sinject, parameters);
			InjectDependencies(sinject, instance, injectionFields, injectionProperties);
			return instance;
		}
	}
}
