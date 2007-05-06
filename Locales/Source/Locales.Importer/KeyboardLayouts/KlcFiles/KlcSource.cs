// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Bnoerj.Locales.Importer.KeyboardLayouts.KlcFiles
{
	/// <summary>
	/// The KeyboardLayoutSource provides methods and properties to access
	/// Microsoft Keyboard Layout Creator files.
	/// </summary>
	/// <remarks>LIGATURE are not supported currently.</remarks>
	internal class KlcSource
	{
		enum KeyWordIndeces
		{
			KBD,
			VERSION,
			COPYRIGHT,
			COMPANY,
			LOCALEID,
			MODIFIERS,
			SHIFTSTATE,
			ATTRIBUTES,
			LAYOUT,
			DEADKEY,
			LIGATURE,
			KEYNAME,
			KEYNAME_EXT,
			KEYNAME_DEAD,
			DESCRIPTIONS,
			ENDKBD
		}
		static readonly String[] keyWords = new String[] { "KBD", "VERSION", "COPYRIGHT", "COMPANY", "LOCALEID", "MODIFIERS", "SHIFTSTATE", "ATTRIBUTES", "LAYOUT", "DEADKEY", "LIGATURE", "KEYNAME", "KEYNAME_EXT", "KEYNAME_DEAD", "DESCRIPTIONS", "ENDKBD" };
		static readonly char[] trimChars = new char[] { ' ', '"', '\r', '\n' };

		String content;
		String localeId;
		String[] attributes;
		Byte[] shiftState;

		/// <summary>
		/// Initializes a new instance of the KeyboardLayoutSource class.
		/// </summary>
		/// <param name="content">The content of a KLC file.</param>
		internal KlcSource(String content)
		{
			this.content = content;

			StripComments();
			NormalizeWhiteSpace();
			if (HasSupportedVersion() == false)
			{
				throw new InvalidContentException("VERSION is not supported.");
			}
		}

		/// <summary>
		/// Gets the value of the LOCALEID block.
		/// </summary>
		internal String LocaleID
		{
			get
			{
				if (String.IsNullOrEmpty(localeId) == true)
				{
					localeId = GetSingleLineBlock("LOCALEID").PadLeft(8, '0');
				}
				return localeId;
			}
		}

		/// <summary>
		/// Gets the ATTRIBUTES block.
		/// </summary>
		internal String[] Attributes
		{
			get
			{
				if (attributes == null)
				{
					attributes = GetMultiLinesBlock("ATTRIBUTES");
				}
				return attributes;
			}
		}

		/// <summary>
		/// Gets the SHIFTSTATE block.
		/// </summary>
		internal Byte[] ShiftState
		{
			get
			{
				if (shiftState == null)
				{
					List<Byte> ss = new List<Byte>();
					String[] lines = GetMultiLinesBlock("SHIFTSTATE");
					foreach (String line in lines)
					{
						int index = line.IndexOf(';');
						if (index > -1)
						{
							ss.Add(Byte.Parse(line.Substring(0, index - 1)));
						}
						else
						{
							ss.Add(Byte.Parse(line));
						}
					}
					shiftState = ss.ToArray();
				}
				return shiftState;
			}
		}

		/// <summary>
		/// Gets the LAYOUT block.
		/// </summary>
#if IGNORED
		internal SortedList Layout
		{
			get
			{
				if (this.m_slLayout == null)
				{
					String[] textArray = GetMultiLinesBlock("LAYOUT");
					for (int i = 0; i < textArray.Length; i++)
					{
						string[] textArray2 = textArray[i].Split(new char[] { ' ' });
						if (textArray2.Length >= 2)
						{
							VirtualKeyContent row = null;
							if (textArray2.Length > 2)
							{
								row = new VirtualKeyContent(textArray2[1], textArray2[0]);
								if (textArray2[2].Equals("SGCap"))
								{
									row.CapsInfo = 0;
									row.SGCap = true;
								}
								else
								{
									row.CapsInfo = byte.Parse(textArray2[2], NumberStyles.AllowHexSpecifier, (IFormatProvider)Utilities.s_nfi);
								}
								for (int j = 3; j < textArray2.Length; j++)
								{
									row.AddCharValue(textArray2[j]);
								}
								if (row.SGCap)
								{
									row.AddCharValue((uint)0xffff);
									i++;
									string[] textArray3 = textArray[i].TrimStart(new char[] { ' ' }).Split(new char[] { ' ' });
									for (int k = 3; k < textArray3.Length; k++)
									{
										if (textArray3[k].Length > 0)
										{
											row.AddCharValue(textArray3[k]);
										}
									}
								}
							}
							if (((row != null) && !this.m_slLayout.ContainsKey(row.Vk)) && (textArray2.Length > 2))
							{
								this.m_slLayout.Add(row.Vk, row);
							}
						}
					}
				}
				return this.m_slLayout;
			}
		}
#endif
		/// <summary>
		/// Strips ; and // end-to-line comments form the content.
		/// </summary>
		void StripComments()
		{
			String[] lines = content.Split(new char[] { '\r' });
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Length > 0)
				{
					if (lines[i][0] == ';')
					{
						lines[i] = "";
					}
					else
					{
						int index = lines[i].IndexOf("//");
						if (index > -1)
						{
							lines[i] = lines[i].Substring(0, index - 1);
						}
					}
				}
			}
			content = String.Join("\r", lines);
		}

		/// <summary>
		/// Normalizes white space into single spaces.
		/// </summary>
		void NormalizeWhiteSpace()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(content);
			sb.Replace('\t', ' ');
			for (int length = sb.Length; ; length = sb.Length)
			{
				sb.Replace("  ", " ");
				if (length == sb.Length)
				{
					break;
				}
			}
			content = sb.ToString();
		}

		/// <summary>
		/// Finds the nearest next key word after the given start index.
		/// </summary>
		/// <param name="startIndex">The index into content to start the search off.</param>
		/// <returns>The index into content of the next key word.</returns>
		int NextKeyWord(int startIndex)
		{
			int endIndex = content.Length;
			foreach (String keyWord in keyWords)
			{
				int index = content.IndexOf(keyWord, startIndex);
				if (index > -1 && index < endIndex)
				{
					endIndex = index;
				}
			}
			return endIndex;
		}

		/// <summary>
		/// Checks, if the KLCs VERSION is supported.
		/// </summary>
		/// <returns>true, if the files version is supported, false otherwise.</returns>
		bool HasSupportedVersion()
		{
			return GetSingleLineBlock("VERSION") == "1.0";
			/*bool foundSupportedVersion = false;
			int keyWordLength = "VERSION".Length;
			int index = content.IndexOf("VERSION");
			if (index > -1)
			{
				index += keyWordLength;
				int eol = content.IndexOf('\r', index);
				if (index < eol)
				{
					String version = content.Substring(index, eol - index).Trim(trimChars);
					foundSupportedVersion = version == "1.0";
				}
			}
			return foundSupportedVersion;*/
		}

		String GetSingleLineBlock(String keyWord)
		{
			String line = "";
			int keyWordLength = keyWord.Length;
			int index = content.IndexOf(keyWord);
			if (index > -1)
			{
				index += keyWordLength;
				int eol = content.IndexOf('\r', index);
				if (index < eol)
				{
					line = content.Substring(index, eol - index).Trim(trimChars);
				}
			}
			return line;
		}

		String[] GetMultiLinesBlock(String keyWord)
		{
			List<String> blockLines = new List<String>();
			int keyWordLength = keyWord.Length;
			int index = content.IndexOf(keyWord);
			if (index > -1)
			{
				index += keyWordLength;
				int endIndex = NextKeyWord(index);
				if (index < endIndex)
				{
					String[] lines = content.Substring(index, endIndex - index).Split(new char[] { '\r' });
					for (int i = 0; i < lines.Length; i++)
					{
						String line = lines[i].Trim();
						if (line.Length > 0 && line[0] != ';')
						{
							blockLines.Add(line);
						}
					}
				}
			}
			return blockLines.ToArray();
		}
	}
}
