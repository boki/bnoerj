// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content.Pipeline;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Importer.KeyboardLayouts;
using Bnoerj.Locales.Importer.Fonts;

namespace Bnoerj.Locales.Importer
{
	[ContentProcessor(DisplayName="Locale - Bnoerj")]
	class LocaleProcessor : ContentProcessor<LocaleSource, LocaleContent>
	{
		public override LocaleContent Process(LocaleSource input, ContentProcessorContext context)
		{
			KeyboardLayoutContent klc = ProcessKeyboardLayout(input, context);
			FontContent font = ProcessFont(input, klc, context);
			return new LocaleContent(
						input.CultureInfo,
						klc,
						font
					);
		}

		KeyboardLayoutContent ProcessKeyboardLayout(LocaleSource input, ContentProcessorContext context)
		{
			// Load keyboard layout
			// FIXME: Use localeSource.CultureInfo.KeyboardLayoutId?
			String klid = String.Format("{0:x8}", input.CultureInfo.KeyboardLayoutId);
			context.Logger.LogMessage("Using keyboard layout {0} for locale {1}", klid, input.CultureInfo.Name);
			KeyboardLayoutSource kls = new KeyboardLayoutSource(klid);
			return context.Convert<KeyboardLayoutSource, KeyboardLayoutContent>(kls, "KeyboardLayoutProcessor");
		}

		FontContent ProcessFont(LocaleSource input, KeyboardLayoutContent keyboardLayoutContent, ContentProcessorContext context)
		{
			// Get all code points used by this keyboard layout
			List<char> codePoints = new List<char>();

			VirtualKeyContent[] keys = keyboardLayoutContent.Keys;
			foreach (VirtualKeyContent key in keys)
			{
				if (key == null)
				{
					continue;
				}

				foreach (VirtualKeyValue value in key.ShiftStates)
				{
					if (value != VirtualKeyValue.Empty)
					{
						GetCodePointsFromCharacters(value.Characters.ToCharArray(), codePoints);
					}
				}
			}

			foreach (DeadKey deadKey in keyboardLayoutContent.DeadKeys.Values)
			{
				GetCodePointsFromCharacters(deadKey.CombinedCharacters, codePoints);
			}

			codePoints.Sort();
			context.Logger.LogMessage("Keyboard layout for locale {0} requires {1} code points", input.CultureInfo.Name, codePoints.Count);
			FontSource fs = new FontSource(input.FontName, codePoints.ToArray());
			return context.Convert<FontSource, FontContent>(fs, "FontProcessor");
		}

		void GetCodePointsFromCharacters(IEnumerable<char> characters, List<char> codePoints)
		{
			// FIXME: consider surrogates
			foreach (char c in characters)
			{
				if (char.IsControl(c) == false &&
					codePoints.FindIndex(delegate(char v) { return v == c; }) == -1)
				{
					codePoints.Add(c);
				}
			}
		}
	}
}
