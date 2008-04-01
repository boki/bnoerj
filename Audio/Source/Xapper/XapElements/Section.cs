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
	class Section
	{
		protected ElementCollection elements;

		public Section()
		{
			elements = new ElementCollection();
		}

		public int ElementCount
		{
			get { return elements.Count; }
		}

		/// <summary>
		/// Gets or sets a specified element.
		/// </summary>
		/// <remarks>
		/// Setting an exisitng section element will replace the first one only.
		/// Use TODO to delete all sections.
		/// </remarks>
		/// <param name="element">The element to get or set.</param>
		/// <returns></returns>
		public IElement this[String element]
		{
			get { return elements.Find(delegate(IElement e) { return e.Name == element; }); }
			set
			{
				int i = elements.FindIndex(delegate(IElement e) { return e.Name == element; });
				if (value == null)
				{
					if (i > -1)
					{
						elements.RemoveAt(i);
					}
				}
				else
				{
					if (i > -1)
					{
						elements[i] = value;
					}
					else
					{
						elements.Add(value);
					}
				}
			}
		}

		public ElementCollection Elements
		{
			get { return elements; }
		}

		public ElementCollection.Enumerator GetEnumerator()
		{
			return elements.GetEnumerator();
		}

		public void AddElement(String line)
		{
			if (String.IsNullOrEmpty(line) == true)
			{
				return;
			}

			int i = line.IndexOf('=');
			if (i > -1)
			{
				String element = line.Substring(0, i).Trim();
				String value = line.Substring(i + 1).Trim();
				ScalarElement scalar = new ScalarElement(element, value);
				int index = elements.FindIndex(delegate(IElement e) { return e.Name == element; });
				if (index > -1)
				{
					elements[index] = scalar;
				}
				else
				{
					elements.Add(scalar);
				}
			}
		}

		public void AddSection(String element, Section section)
		{
			elements.Add(new SectionElement(element, section));
		}

		public void Add(IElement element)
		{
			elements.Add(element);
		}

		/// <summary>
		/// Gets all elements and sub-elements with the specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ElementCollection GetElements(String name)
		{
			ElementCollection res = new ElementCollection();
			foreach (IElement element in elements)
			{
				if (name == element.Name)
				{
					res.Add(element);
				}

				Section section = element.Value as Section;
				if (section != null)
				{
					res.AddRange(section.GetElements(name));
				}
			}
			return res;
		}
	}
}
