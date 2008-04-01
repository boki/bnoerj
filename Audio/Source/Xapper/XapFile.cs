// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Bnoerj.Audio.XapElements;
using System.IO;

namespace Bnoerj.Audio
{
	class XapFile
	{
/*
		readonly List<String> sectionNames = new List<String>(new String[] {
			"Options",

			"Global Settings",
				"Category",
					"Parent",
					"Instance Limit",
						"Crossfade",
				"Variable",
				"RPC",
					"Variable Entry",
					"RPC Curve",
						"Effect Parameter Entry",
						"RPC Point",
					"Effect Entry",
				"Effect",
					"Effect Parameter",
				"Codec Preset",

			"Wave Bank",
				"Wave",
					"Cache",

			"Sound Bank",
				"Clip",
				"Sound",
					"Effect Entry",
					"Track",
						"RPC Curve Entry",
						"Play Wave Event",
							"Event Header",
							"Pitch Variation",
							"Volume Variation",
							"Variation",
							"Wave Entry",
						"Play Wave From Offset Event",
						"Play Wave Variation Event",
						"Play Sound Event",
							"Event Header",
							"Variation",
							"Sound Entry",
						"Stop Event",
							"Event Header",
						"Wait Event",
							"Event Header",
							"Conditional",
						"Set Effect Parameter",
						"Set Variable Event",
							"Event Header",
							"Variable Entry",
							"Equation",
							"Recurrence",
						"Set Pitch Event",
							"Event Header",
							"Recurrence",
							"Ramp",
							"Equation",
						"Set Volume Event",
							"Event Header",
							"Recurrence",
							"Ramp",
							"Equation",
						"Set Marker Event",
							"Event Header",
							"Recurrence",
						"Set Variable Recurring Event",
							"Event Header",
						"Set Marker Recurring Event",
							"Event Header",
					"RPC Entry",
				"Cue",
					"Variation",
					"Sound Entry",
					"Transition",
						"Crossfade",
					"Instance Limit",
						"Crossfade",
		});
*/
		Section elements;

		public XapFile()
		{
		}

		public String Signature
		{
			get { return elements["Signature"].Value as String; }
		}

		public String Version
		{
			get { return elements["Version"].Value as String; }
		}

		public String ContentVersion
		{
			get { return elements["Content Version"].Value as String; }
			set { elements["Content Version"].Value = value; }
		}

		public String Release
		{
			get { return elements["Release"].Value as String; }
			set { elements["Release"].Value = value; }
		}

		public ElementCollection Elements
		{
			get { return elements.Elements; }
		}

		public IElement this[String element]
		{
			get { return elements[element]; }
		}

		public ElementCollection GetElements(String name)
		{
			return elements.GetElements(name);
		}

		//REVIEW: Might be better to pass a filename/stream
		public void Open(String source)
		{
			elements = new Section();

			//REVIEW: Need to check for colons and newlines in comments
			char[] split = new char[] { ';', '\n' };

			Stack<Section> sections = new Stack<Section>();

			String sectionType = "";
			Section curSection = elements;

			int i = 0;
			while (i < source.Length)
			{
				int j = source.IndexOfAny(split, i);
				if (j == -1)
				{
					break;
				}

				String line = source.Substring(i, j - i).Trim();
				if (source[j] == ';')
				{
					curSection.AddElement(line);
				}
				else if (line.Length > 0)
				{
					char firstChar = line[0];
					if (firstChar == '{')
					{
						sections.Push(curSection);
						Section newSection = new Section();
						curSection.AddSection(sectionType, newSection);
						curSection = newSection;
					}
					else if (firstChar == '}')
					{
						curSection = sections.Pop();
					}
					else
					{
						sectionType = line;
					}
				}

				i = j + 1;
			}
		}

		public void Save(Stream stream)
		{
			StringBuilder sb = new StringBuilder();
			WriteSection(elements, stream, ref sb, 0);
		}

		void WriteSection(Section section, Stream stream, ref StringBuilder sb, int indent)
		{
			String indentChars = "".PadLeft(4 * indent);
			foreach (IElement element in section)
			{
				ScalarElement scalar = element as ScalarElement;
				if (scalar != null)
				{
					sb.AppendFormat("{0}{1} = {2};\r\n", indentChars, element.Name, scalar.Value);
				}

				SectionElement se = element as SectionElement;
				if (se != null)
				{
					Section subSection = se.Value;
					sb.AppendFormat("\r\n{0}{1}\r\n{0}{{\r\n", indentChars, element.Name);
					WriteSection(subSection, stream, ref sb, indent + 1);
					sb.AppendFormat("{0}}}\r\n", indentChars);
				}
			}

			WriteStringBuilder(stream, ref sb);
		}

		void WriteStringBuilder(Stream stream, ref StringBuilder sb)
		{
			//REVIEW: what encoding does XACT use?
			byte[] buffer = Encoding.ASCII.GetBytes(sb.ToString());
			stream.Write(buffer, 0, buffer.Length);

			sb = new StringBuilder();
		}
	}
}
