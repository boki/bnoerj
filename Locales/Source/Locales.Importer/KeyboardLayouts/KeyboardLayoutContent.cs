// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Bnoerj.Locales.KeyboardLayouts;

namespace Bnoerj.Locales.Importer.KeyboardLayouts
{
	internal class KeyboardLayoutContent
	{
		String layoutId;
		KeysEx specialShiftVk;
		VirtualKeyContent[] keys;
		Dictionary<char, DeadKey> deadKeys;

		internal KeyboardLayoutContent(String layoutId)
		{
			this.layoutId = layoutId;
			this.specialShiftVk = KeysEx.None;
			this.keys = new VirtualKeyContent[256];
			this.deadKeys = null;
		}

		internal String LayoutId
		{
			get { return layoutId; }
		}

		internal KeysEx SpecialShiftVk
		{
			get { return specialShiftVk; }
			set { specialShiftVk = value; }
		}

		internal ShiftState MaxShiftState
		{
			get { return (specialShiftVk == KeysEx.None ? ShiftState.ShftMenuCtrl : ShiftState.ShftSpcl); }
		}

		internal VirtualKeyContent[] Keys
		{
			get { return keys; }
			set { keys = value; }
		}

		internal Dictionary<char, DeadKey> DeadKeys
		{
			get { return deadKeys; }
			set { deadKeys = value; }
		}
	}
}
