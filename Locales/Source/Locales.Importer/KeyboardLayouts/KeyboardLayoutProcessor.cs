// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content.Pipeline;
using Bnoerj.Locales.KeyboardLayouts;

namespace Bnoerj.Locales.Importer.KeyboardLayouts
{
	[ContentProcessor(DisplayName = "KeyboardLayout - Bnoerj")]
	class KeyboardLayoutProcessor : ContentProcessor<KeyboardLayoutSource, KeyboardLayoutContent>
	{
		public override KeyboardLayoutContent Process(KeyboardLayoutSource input, ContentProcessorContext context)
		{
			// Load keyboard layout
			IntPtr hkl = NativeMethods.User32.LoadKeyboardLayout(input.LayoutId, NativeMethods.User32.KLF_NOTELLSHELL);
			if (hkl == IntPtr.Zero)
			{
				throw new InvalidContentException(String.Format("Keyboard layout {0} not found", input.LayoutId));
			}

			// 
			KeyboardLayoutContent kbl = new KeyboardLayoutContent(input.LayoutId);

			InitializeScanCodes(hkl, kbl.Keys);
			FindSpecialShiftState(hkl, kbl);
			ProcessKeys(hkl, kbl);

			// Check, if the keyboard layout can be unloaded
			InputLanguageCollection ilc = InputLanguage.InstalledInputLanguages;
			foreach (InputLanguage il in ilc)
			{
				if (hkl == il.Handle)
				{
					hkl = IntPtr.Zero;
					break;
				}
			}
			if (hkl != IntPtr.Zero)
			{
				NativeMethods.User32.UnloadKeyboardLayout(hkl);
			}

			return kbl;
		}

		void InitializeScanCodes(IntPtr hkl, VirtualKeyContent[] keys)
		{
			// Scroll through the Scan Code (SC) values and get the valid Virtual Key (VK)
			// values in it. Then, store the SC in each valid VK so it can act as both a 
			// flag that the VK is valid, and it can store the SC value.
			for (uint sc = 0x01; sc <= 0x7f; sc++)
			{
				VirtualKeyContent key = new VirtualKeyContent(hkl, sc);
				if (key.VK != 0)
				{
					keys[(uint)key.VK] = key;
				}
			}

			// add the special keys that do not get added from the code above
			for (KeysEx ke = KeysEx.VK_NUMPAD0; ke <= KeysEx.VK_NUMPAD9; ke++)
			{
				keys[(uint)ke] = new VirtualKeyContent(hkl, ke);
			}
			keys[(uint)KeysEx.VK_DIVIDE] = new VirtualKeyContent(hkl, KeysEx.VK_DIVIDE);
			keys[(uint)KeysEx.VK_CANCEL] = new VirtualKeyContent(hkl, KeysEx.VK_CANCEL);
			keys[(uint)KeysEx.VK_DECIMAL] = new VirtualKeyContent(hkl, KeysEx.VK_DECIMAL);
		}

		void FindSpecialShiftState(IntPtr hkl, KeyboardLayoutContent kbl)
		{
			// See if there is a special shift state added
			for (KeysEx vk = KeysEx.None; vk <= KeysEx.Last; vk++)
			{
				uint sc = NativeMethods.User32.MapVirtualKeyEx((uint)vk, 0, hkl);
				uint vkL = NativeMethods.User32.MapVirtualKeyEx(sc, 1, hkl);
				uint vkR = NativeMethods.User32.MapVirtualKeyEx(sc, 3, hkl);
				if (vkL != vkR && (uint)vk != vkL)
				{
					switch (vk)
					{
					case KeysEx.VK_LCONTROL:
					case KeysEx.VK_RCONTROL:
					case KeysEx.VK_LSHIFT:
					case KeysEx.VK_RSHIFT:
					case KeysEx.VK_LMENU:
					case KeysEx.VK_RMENU:
						break;

					default:
						kbl.SpecialShiftVk = vk;
						break;
					}
				}
			}
		}

		void ProcessKeys(IntPtr hkl, KeyboardLayoutContent kbl)
		{
			Dictionary<char, DeadKey> deadKeys = new Dictionary<char, DeadKey>();
			KeysEx[] lpKeyState = new KeysEx[256];

			for (uint iKey = 0; iKey < kbl.Keys.Length; iKey++)
			{
				if (kbl.Keys[iKey] != null)
				{
					for (ShiftState ss = ShiftState.Base; ss <= kbl.MaxShiftState; ss++)
					{
						ProcessKeyShiftState(hkl, lpKeyState, iKey, ss, kbl, deadKeys);
					}
				}
			}

			kbl.DeadKeys = deadKeys;
		}

		void ProcessKeyShiftState(IntPtr hkl, KeysEx[] lpKeyState, uint iKey, ShiftState ss, KeyboardLayoutContent kbl, Dictionary<char, DeadKey> deadKeys)
		{
			// Skip Alt and Shift+Alt because they ain't supported
			if (ss == ShiftState.Menu || ss == ShiftState.ShftMenu)
			{
				return;
			}

			// Scratchpad used in many places
			StringBuilder sbBuffer;
			for (int caps = 0; caps <= 1; caps++)
			{
				Utilities.ClearKeyboardBuffer((uint)KeysEx.VK_DECIMAL, kbl.Keys[(uint)KeysEx.VK_DECIMAL].SC, hkl);
				Utilities.FillKeyState(kbl, lpKeyState, ss, (caps == 0));
				sbBuffer = new StringBuilder(10);
				int rc = NativeMethods.User32.ToUnicodeEx((uint)kbl.Keys[iKey].VK, kbl.Keys[iKey].SC, lpKeyState, sbBuffer, sbBuffer.Capacity, 0, hkl);
				if (rc > 0)
				{
					if (sbBuffer.Length == 0)
					{
						// Someone defined NULL on the keyboard; let's coddle them
						kbl.Keys[iKey].SetShiftState(ss, "\u0000", false, (caps == 0));
					}
					else
					{
						if ((rc == 1) &&
							(ss == ShiftState.Ctrl || ss == ShiftState.ShftCtrl) &&
							((uint)kbl.Keys[iKey].VK == ((uint)sbBuffer[0] + 0x40)))
						{
							// ToUnicodeEx has an internal knowledge about those 
							// VK_A ~ VK_Z keys to produce the control characters, 
							// when the conversion rule is not provided in keyboard 
							// layout files
							continue;
						}
						kbl.Keys[iKey].SetShiftState(ss, sbBuffer.ToString().Substring(0, rc), false, (caps == 0));
					}
				}
				else if (rc < 0)
				{
					kbl.Keys[iKey].SetShiftState(ss, sbBuffer.ToString().Substring(0, 1), true, (caps == 0));

					// It's a dead key; let's flush out whats stored in the keyboard state.
					Utilities.ClearKeyboardBuffer((uint)KeysEx.VK_DECIMAL, kbl.Keys[(uint)KeysEx.VK_DECIMAL].SC, hkl);
					char ch = kbl.Keys[iKey].GetShiftState(ss, caps == 0).Characters[0];
					if (deadKeys.ContainsKey(ch) == false)
					{
						DeadKey dk = Utilities.ProcessDeadKey(kbl, iKey, ss, lpKeyState, kbl.Keys, caps == 0, hkl);
						deadKeys.Add(ch, dk);
					}
				}
			}
		}
	}
}
