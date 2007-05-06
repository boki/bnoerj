// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Text;

namespace Bnoerj.Locales
{
	/// <summary>
	/// The Locale provides access to a locales keyboard layout and font.
	/// </summary>
	public class Locale : IDisposable
	{
		CultureInfo cultureInfo;
		KeyboardLayout keyboardLayout;
		Font font;
		bool disposed;

		/// <summary>
		/// Initializes a new instance of Locale. For use with FontReader only.
		/// </summary>
		/// <param name="cultureInfo"></param>
		/// <param name="keyboardLayout"></param>
		/// <param name="font"></param>
		internal Locale(CultureInfo cultureInfo, KeyboardLayout keyboardLayout, Font font)
		{
			this.cultureInfo = cultureInfo;
			this.keyboardLayout = keyboardLayout;
			this.font = font;
		}

		/// <summary>
		/// Gets the culture information of the Locale.
		/// </summary>
		public CultureInfo CultureInfo
		{
			get { return cultureInfo; }
		}

		/// <summary>
		/// Gets the KeyboardLayout of the Locale.
		/// </summary>
		public KeyboardLayout KeyboardLayout
		{
			get { return keyboardLayout; }
		}

		/// <summary>
		/// Gets the Font to draw keyboard input using the Locale.
		/// </summary>
		public Font Font
		{
			get { return font; }
		}

		public bool IsDisposed
		{
			get { return disposed; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (disposed == false)
			{
				if (font != null && font.IsDisposed == false)
				{
					font.Dispose();
				}
			}
			disposed = true;
		}
	}
}
