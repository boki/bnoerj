// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;

namespace Bnoerj.Locales.KeyboardLayouts
{
	/// <summary>
	/// Defines shift states.
	/// </summary>
	public enum ShiftState : int
	{
		/// <summary>Normal key.</summary>
		Base            = 0,
		/// <summary>Shift key is pressed.</summary>
		Shft            = 1,
		/// <summary>Control is pressed.</summary>
		Ctrl            = 2,
		/// <summary>Shift+Control keys are pressed.</summary>
		ShftCtrl        = Shft | Ctrl,
		/// <summary>Alt key is pressed.</summary>
		/// <remarks>Not used.</remarks>
		Menu            = 4,
		/// <summary>Shift+Alt keys are pressed.</summary>
		/// <remarks>Not used.</remarks>
		ShftMenu        = Shft | Menu,
		/// <summary>Control+Alt keys are pressed.</summary>
		MenuCtrl        = Menu | Ctrl,
		/// <summary>Shift+Control+Alt keys are pressed.</summary>
		ShftMenuCtrl    = Shft | Menu | Ctrl,
		/// <summary>Special shift state key is pressed.</summary>
		Spcl            = 8,
		/// <summary>Shift+Special shift state keys are pressed</summary>
		ShftSpcl        = Shft | Spcl,
	}
}
