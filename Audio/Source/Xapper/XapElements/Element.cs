// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;

namespace Bnoerj.Audio.XapElements
{
	class Element<T> : IElement
	{
		static readonly Element<T> Empty = new Element<T>();

		String name;
		T value;

		Element()
		{
			name = null;
			value = default(T);
		}

		public Element(String name, T value)
		{
			this.name = name;
			this.value = value;
		}

		public String Name
		{
			get { return name; }
		}

		Object IElement.Value
		{
			get { return value; }
			set { this.value = (T)value; }
		}

		public T Value
		{
			get { return value; }
			set { this.value = value; }
		}
	}
}
