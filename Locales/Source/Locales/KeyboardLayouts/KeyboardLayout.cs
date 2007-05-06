// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace Bnoerj.Locales.KeyboardLayouts
{
	/*
	 * If you think about the consequences of that notion -- that you explicitly
	 * tell a key that it is a dead key and it will then look up its own
	 * individual dead key table on the next keystoke. Such an architecture
	 * goes a long way to explaining the reason why you must have a valid
	 * character at each stage of a chained dead key -- because once you jump
	 * to a new "dead key table" there is no state information about the old
	 * "dead key table". And since the dead key tables only allow a single
	 * UTF-16 code unit for the base character and one more for the combined
	 * character, there is simply nowhere to store the knowledge or the need
	 * for additional characters.
	 * 
	 * http://blogs.msdn.com/michkap/archive/2006/04/22/581107.aspx
	 * */
	public class KeyboardLayout
	{
		String layoutId;
		KeysEx specialShiftVk;
		VirtualKey[] keys;
		Dictionary<char, DeadKey> deadKeys;

		internal KeyboardLayout(String layoutId, KeysEx specialShiftVk, VirtualKey[] keys, Dictionary<char, DeadKey> deadKeys)
		{
			this.layoutId = layoutId;
			this.specialShiftVk = specialShiftVk;
			this.keys = keys;
			this.deadKeys = deadKeys;
		}

		public String LayoutId
		{
			get { return layoutId; }
		}

		public KeysEx SpecialShiftVk
		{
			get { return specialShiftVk; }
		}

		public ShiftState MaxShiftState
		{
			get { return (specialShiftVk == KeysEx.None ? ShiftState.ShftMenuCtrl : ShiftState.ShftSpcl); }
		}

		public VirtualKey[] Keys
		{
			get { return keys; }
		}

		public Dictionary<char, DeadKey> DeadKeys
		{
			get { return deadKeys; }
		}

		//TODO: Translate scan code/Microsoft.Xna.Framework.Input.Keys to KeysEx
		public VirtualKeyValue[] ProcessKeys(KeyboardState state)
		{
			return ProcessKeys(state, ProcessKeysFlags.Default);
		}

		public VirtualKeyValue[] ProcessKeys(KeyboardState state, ProcessKeysFlags flags)
		{
			ShiftState shiftState =
				(state.IsKeyDown(XnaKeys.LeftShift) || state.IsKeyDown(XnaKeys.RightShift) ? ShiftState.Shft : 0) |
				(state.IsKeyDown(XnaKeys.LeftControl) || state.IsKeyDown(XnaKeys.RightControl) ? ShiftState.Ctrl : 0) |
				//FIXME: RightAlt may be specialShiftVk, even though ShiftState.Menu is not supported in keyboard layouts
				(state.IsKeyDown(XnaKeys.LeftAlt) || state.IsKeyDown(XnaKeys.RightAlt) ? ShiftState.Menu : 0) |
				//FIXME: ensure that XnaKeys == KeysEx
				(specialShiftVk != KeysEx.None ? state.IsKeyDown((XnaKeys)specialShiftVk) ? ShiftState.Spcl : 0 : 0);
			bool caps = (flags & ProcessKeysFlags.IgnoreCapsLock) != 0 ? false : state.IsKeyDown(XnaKeys.CapsLock);
#if IGNORED
			XnaKeys[] keys = state.GetPressedKeys();
			KeysEx[] pressedKeys = new KeysEx[keys.Length];
			// Filter out control keys
			int i = 0;
			foreach (XnaKeys key in keys)
			{
				VirtualKey vk = keys[(int)key];
				if (vk != null && char.IsControl(vk.GetShiftState(ShiftState.Base, false).Characters[0]) == false)
				{
					//FIXME: ensure that XnaKeys == KeysEx
					pressedKeys[i++] = (KeysEx)key;
				}
			}
#else
			XnaKeys[] pressedKeys = state.GetPressedKeys();
#endif
			List<VirtualKeyValue> res = new List<VirtualKeyValue>(pressedKeys.Length);
			foreach (XnaKeys key in pressedKeys)
			{
				VirtualKey vk = keys[(int)key];
				if (vk != null)
				{
					res.Add(vk.GetShiftState(shiftState, caps));
				}
			}
			return res.ToArray();
		}
	}
}
