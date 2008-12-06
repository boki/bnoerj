// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Bnoerj.Sinject
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Exception that is thrown if the dependency is missing and the behavior
	/// is set to Throw.
	/// </summary>
	[Serializable]
	public class DependencyMissingException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the DependencyMissingException class.
		/// </summary>
		public DependencyMissingException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the DependencyMissingException class
		/// using the specified exception message.
		/// </summary>
		/// <param name="message">Exception message.</param>
		public DependencyMissingException(String message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DependencyMissingException class
		/// using the specified exception message and inner exception.
		/// </summary>
		/// <param name="message">Exception message.</param>
		/// <param name="inner">Inner exception.</param>
		public DependencyMissingException(String message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DependencyMissingException class
		/// using the serialization information and streaming context.
		/// </summary>
		/// <param name="info">Streaming context.</param>
		/// <param name="context">Serialization context.</param>
		protected DependencyMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
