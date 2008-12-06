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
	/// Exception that is thrown if no suitable constructor is found.
	/// </summary>
	[Serializable]
	public class NoSuitableConstructorFoundException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the NoSuitableConstructorFoundException
		/// class.
		/// </summary>
		public NoSuitableConstructorFoundException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the NoSuitableConstructorFoundException
		/// class using the specified message.
		/// </summary>
		/// <param name="message">Exception message.</param>
		public NoSuitableConstructorFoundException(String message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the NoSuitableConstructorFoundException
		/// class the specified message and inner exception.
		/// </summary>
		/// <param name="message">Exception message.</param>
		/// <param name="inner">Inner exception.</param>
		public NoSuitableConstructorFoundException(String message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the NoSuitableConstructorFoundException
		/// class using the serialization information and streaming context.
		/// </summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		protected NoSuitableConstructorFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
