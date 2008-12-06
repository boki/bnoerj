// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Bnoerj.Sinject.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Provides methods and properties for all activators.
	/// </summary>
	internal abstract class ActivatorBase
	{
		/// <summary>
		/// The concrete type the activator will construct.
		/// </summary>
		Type concreteType;

		/// <summary>
		/// Initializes a new instance of the ActivatorBase class using the
		/// specified concrete type.
		/// </summary>
		/// <param name="concreteType">The type this instance should ...</param>
		public ActivatorBase(Type concreteType)
		{
			this.concreteType = concreteType;
		}

		/// <summary>
		/// Gets an instance of the type.
		/// </summary>
		/// <param name="sinject">The resolver to use to inject dependencies.</param>
		/// <param name="parameters">Constructor parameters.</param>
		/// <returns>An instance of the type.</returns>
		public abstract Object GetInstance(Sinject sinject, params Object[] parameters);

		/// <summary>
		/// Constructs a new instance of the type.
		/// </summary>
		/// <param name="sinject">A Sinject instance to use to resolve dependency types.</param>
		/// <param name="parameters">Optional constructor parameters to use when no injection constructor is defined.</param>
		/// <returns>An instance of the type.</returns>
		/// <exception cref="DependencyMissingException"></exception>
		/// <exception cref="NoSuitableConstructorFoundException"></exception>
		protected Object ConstructInstance(Sinject sinject, params Object[] parameters)
		{
			ConstructorInfo constructorInfo = GetInjectionConstructor();
			if (constructorInfo != null)
			{
				ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
				Object[] injectedParameters = new Object[parameterInfos.Length];

				for (int i = 0; i < parameterInfos.Length; i++)
				{
					var parameterInfo = parameterInfos[i];
					var missingBehavior = DependencyMissingBehavior.Null;
					InjectAttribute[] injectAttributes = parameterInfo.GetCustomAttributes(typeof(InjectAttribute), false) as InjectAttribute[];
					if (injectAttributes.Length > 0)
					{
						missingBehavior = injectAttributes[0].DependencyMissingBehavior;
					}

					injectedParameters[i] = sinject.Resolve(parameterInfo.ParameterType);
					if (injectedParameters[i] == null && missingBehavior == DependencyMissingBehavior.Throw)
					{
						throw new DependencyMissingException();
					}
				}

				return constructorInfo.Invoke(injectedParameters);
			}

			constructorInfo = GetDefaultConstructor(parameters);
			if (constructorInfo != null)
			{
				return constructorInfo.Invoke(parameters);
			}

			throw new NoSuitableConstructorFoundException();
		}

		/// <summary>
		/// Gets all fields of the type marked with the InjectAttribute.
		/// </summary>
		/// <returns>An array of FieldInfo.</returns>
		protected FieldInfo[] GetInjectionFields()
		{
			FieldInfo[] fieldInfos = concreteType.GetFields();
			List<FieldInfo> injectionFields = new List<FieldInfo>();
			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				if (fieldInfo.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0)
				{
					injectionFields.Add(fieldInfo);
				}
			}

			return injectionFields.ToArray();
		}

		/// <summary>
		/// Gets all properties of the type marked with the InjectAttribute.
		/// </summary>
		/// <returns>An array of PropertyInfo.</returns>
		protected PropertyInfo[] GetInjectionProperties()
		{
			PropertyInfo[] propertyInfos = concreteType.GetProperties();
			List<PropertyInfo> injectionProperties = new List<PropertyInfo>();
			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				if (propertyInfo.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0)
				{
					injectionProperties.Add(propertyInfo);
				}
			}

			return injectionProperties.ToArray();
		}

		/// <summary>
		/// Injects the dependencies into the specified fields and properties
		/// of the specified target using the specified Sinject instance to
		/// resolve the types.
		/// </summary>
		/// <param name="sinject">The sinjkect instance to use to resolve types.</param>
		/// <param name="target">The target object.</param>
		/// <param name="fields">The fields to inject into.</param>
		/// <param name="properties">The properties to inject into.</param>
		protected void InjectDependencies(Sinject sinject, Object target, FieldInfo[] fields, PropertyInfo[] properties)
		{
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(target, sinject.Resolve(fieldInfo.FieldType));
			}

			foreach (PropertyInfo propertyInfo in properties)
			{
				propertyInfo.SetValue(target, sinject.Resolve(propertyInfo.PropertyType), sinject.EmptyObjectArray);
			}
		}

		/// <summary>
		/// Gets the constructor marked with the InjectAttribute.
		/// </summary>
		/// <returns>A ConstructorInfo or null.</returns>
		ConstructorInfo GetInjectionConstructor()
		{
			ConstructorInfo[] constructorInfos = concreteType.GetConstructors();
			foreach (ConstructorInfo ci in constructorInfos)
			{
				if (ci.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0)
				{
					return ci;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the specified types constructor that matches the parameters.
		/// </summary>
		/// <param name="parameters">Parameters to use when choosing a constructor.</param>
		/// <returns>A ConstructorInfo matching the parameters or null.</returns>
		ConstructorInfo GetDefaultConstructor(Object[] parameters)
		{
			Type[] parameterTypes = new Type[parameters.Length];
			for (var i = 0; i < parameters.Length; i++)
			{
				parameterTypes[i] = parameters[i].GetType();
			}

			return concreteType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public,
				null,
				parameterTypes,
				null);
		}
	}
}
