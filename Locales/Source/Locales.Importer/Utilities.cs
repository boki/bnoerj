// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Importer.KeyboardLayouts;

namespace Bnoerj.Locales.Importer
{
	internal static class Utilities
	{
		static KeysEx[] lpKeyStateNull = new KeysEx[256];

		internal static void ClearKeyboardBuffer(uint vk, uint sc, IntPtr hkl)
		{
			StringBuilder sb = new StringBuilder(10);
			int rc = 0;
			while (rc != 1)
			{
				rc = NativeMethods.User32.ToUnicodeEx(vk, sc, lpKeyStateNull, sb, sb.Capacity, 0, hkl);
			}
		}

		internal static void FillKeyState(KeyboardLayoutContent kbl, KeysEx[] lpKeyState, ShiftState ss, bool fCapsLock)
		{
			lpKeyState[(int)KeysEx.VK_SHIFT] = (((ss & ShiftState.Shft) != 0) ? (KeysEx)0x80 : (KeysEx)0x00);
			lpKeyState[(int)KeysEx.VK_CONTROL] = (((ss & ShiftState.Ctrl) != 0) ? (KeysEx)0x80 : (KeysEx)0x00);
			lpKeyState[(int)KeysEx.VK_MENU] = (((ss & ShiftState.Menu) != 0) ? (KeysEx)0x80 : (KeysEx)0x00);
			if (kbl.SpecialShiftVk != KeysEx.None)
			{
				// The Xxxx key has been assigned, so let's include it
				lpKeyState[(int)kbl.SpecialShiftVk] = (((ss & ShiftState.Spcl) != 0) ? (KeysEx)0x80 : (KeysEx)0x00);
			}
			lpKeyState[(int)KeysEx.VK_CAPITAL] = (fCapsLock ? (KeysEx)0x01 : (KeysEx)0x00);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="iKeyDead">The index into the VirtualKey of the dead key</param>
		/// <param name="shiftStateDead">The shiftstate that contains the dead key</param>
		/// <param name="lpKeyStateDead">The key state for the dead key</param>
		/// <param name="rgKey">Our array of dead keys</param>
		/// <param name="fCapsLock">Was the caps lock key pressed?</param>
		/// <param name="hkl">The keyboard layout</param>
		/// <returns></returns>
		internal static DeadKey ProcessDeadKey(KeyboardLayoutContent kbl, uint iKeyDead, ShiftState shiftStateDead, KeysEx[] lpKeyStateDead, VirtualKeyContent[] rgKey, bool fCapsLock, IntPtr hkl)
		{
			KeysEx[] lpKeyState = new KeysEx[256];
			String dkShiftState = rgKey[iKeyDead].GetShiftState(shiftStateDead, fCapsLock).Characters;
			DeadKey deadKey = new DeadKey(dkShiftState[0]);

			for (uint iKey = 0; iKey < rgKey.Length; iKey++)
			{
				if (rgKey[iKey] != null)
				{
					StringBuilder sbBuffer = new StringBuilder(10);     // Scratchpad we use many places

					for (ShiftState ss = ShiftState.Base; ss <= kbl.MaxShiftState; ss++)
					{
						int rc = 0;
						if (ss == ShiftState.Menu || ss == ShiftState.ShftMenu)
						{
							// Alt and Shift+Alt don't work, so skip them
							continue;
						}

						for (int caps = 0; caps <= 1; caps++)
						{
							// First the dead key
							while (rc >= 0)
							{
								// We know that this is a dead key coming up, otherwise
								// this function would never have been called. If we do
								// *not* get a dead key then that means the state is 
								// messed up so we run again and again to clear it up.
								// Risk is technically an infinite loop but per Hiroyama
								// that should be impossible here.
								rc = NativeMethods.User32.ToUnicodeEx((uint)rgKey[iKeyDead].VK, rgKey[iKeyDead].SC, lpKeyStateDead, sbBuffer, sbBuffer.Capacity, 0, hkl);
							}

							// Now fill the key state for the potential base character
							FillKeyState(kbl, lpKeyState, ss, (caps != 0));

							sbBuffer = new StringBuilder(10);
							rc = NativeMethods.User32.ToUnicodeEx((uint)rgKey[iKey].VK, rgKey[iKey].SC, lpKeyState, sbBuffer, sbBuffer.Capacity, 0, hkl);
							if (rc == 1)
							{
								// That was indeed a base character for our dead key.
								// And we now have a composite character. Let's run
								// through one more time to get the actual base 
								// character that made it all possible?
								char combchar = sbBuffer[0];
								sbBuffer = new StringBuilder(10);
								rc = NativeMethods.User32.ToUnicodeEx((uint)rgKey[iKey].VK, rgKey[iKey].SC, lpKeyState, sbBuffer, sbBuffer.Capacity, 0, hkl);

								char basechar = sbBuffer[0];

								if (deadKey.DeadCharacter == combchar)
								{
									// Since the combined character is the same as the dead key,
									// we must clear out the keyboard buffer.
									ClearKeyboardBuffer((uint)KeysEx.VK_DECIMAL, rgKey[(uint)KeysEx.VK_DECIMAL].SC, hkl);
								}

								if ((((ss == ShiftState.Ctrl) || (ss == ShiftState.ShftCtrl)) && (char.IsControl(basechar))) || (basechar.Equals(combchar)))
								{
									// ToUnicodeEx has an internal knowledge about those 
									// VK_A ~ VK_Z keys to produce the control characters, 
									// when the conversion rule is not provided in keyboard 
									// layout files

									// Additionally, dead key state is lost for some of these
									// character combinations, for unknown reasons.

									// Therefore, if the base character and combining are equal,
									// and its a CTRL or CTRL+SHIFT state, and a control character
									// is returned, then we do not add this "dead key" (which
									// is not really a dead key).
									continue;
								}

								if (!deadKey.ContainsBaseCharacter(basechar))
								{
									deadKey.AddDeadKeyRow(basechar, combchar);
								}
							}
							else if (rc > 1)
							{
								// Not a valid dead key combination, sorry! We just ignore it.
							}
							else if (rc < 0)
							{
								// It's another dead key, so we ignore it (other than to flush it from the state)
								ClearKeyboardBuffer((uint)KeysEx.VK_DECIMAL, rgKey[(uint)KeysEx.VK_DECIMAL].SC, hkl);
							}
						}
					}
				}
			}
			return deadKey;
		}

		internal static SortedList<uint, KeyboardLayoutInfo> KeyboardsOnMachine()
		{
			InputLanguageCollection installedInputLanguages = InputLanguage.InstalledInputLanguages;
			RegistryKey keyLayouts = null;
			RegistryKey keyLayout = null;
			SortedList<uint, KeyboardLayoutInfo> list = null;
			NumberFormatInfo nfi = new NumberFormatInfo();

			try
			{
				keyLayouts = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts", false);
				if (keyLayouts == null)
				{
					throw new SecurityException();
				}

				list = new SortedList<uint, KeyboardLayoutInfo>(keyLayouts.SubKeyCount);
				foreach (String strKLID in keyLayouts.GetSubKeyNames())
				{
					keyLayout = keyLayouts.OpenSubKey(strKLID, false);
					String layoutName = "";
					String layoutText = keyLayout.GetValue("Layout Text", "").ToString();
					String pszSource = keyLayout.GetValue("Layout Display Name", "").ToString();
					if ((pszSource != null) && (pszSource.Length > 0))
					{
						StringBuilder pszOutBuf = new StringBuilder(260);
						if (NativeMethods.Shlwapi.SHLoadIndirectString(pszSource, pszOutBuf, (uint)pszOutBuf.Capacity, IntPtr.Zero) == 0)
						{
							layoutName = pszOutBuf.ToString();
						}
					}
					if (layoutName.Length == 0)
					{
						layoutName = layoutText;
					}

					if (layoutName.Length > 0 || layoutText.Length > 0)
					{
						uint nKLID = uint.Parse(strKLID, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

						// Ignore 0x5FE and Input Method Editor (IME) layouts
						if ((nKLID & 0xFFF) == 0x5FE || nKLID >= 0xE0000000)
						{
							continue;
						}

						bool doUnloadKeyboardLayout = true;
						IntPtr hkl = NativeMethods.User32.LoadKeyboardLayout(nKLID.ToString("x8", nfi), 0x80);
						if (hkl == IntPtr.Zero)
						{
							continue;
						}

						foreach (InputLanguage language in installedInputLanguages)
						{
							if (hkl == language.Handle)
							{
								doUnloadKeyboardLayout = false;
								break;
							}
						}
						if (doUnloadKeyboardLayout)
						{
							NativeMethods.User32.UnloadKeyboardLayout(hkl);
						}

						KeyboardLayoutInfo keyboard = new KeyboardLayoutInfo();
						keyboard.HKL = nKLID;
						keyboard.LayoutFile = keyLayout.GetValue("Layout File", "").ToString();
						keyboard.LayoutText = layoutText;
						keyboard.LayoutName = layoutName;
						String s = keyLayout.GetValue("Layout Id", "").ToString();
						if (s.Length > 0)
						{
							keyboard.LayoutID = ushort.Parse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
						}
						list.Add(nKLID, keyboard);
					}
					keyLayout.Close();
					keyLayout = null;
				}
			}
			catch (ArgumentException)
			{
				list = null;
			}
			catch (SecurityException)
			{
				list = null;
			}
			finally
			{
				if (keyLayout != null)
				{
					keyLayout.Close();
				}
				if (keyLayouts != null)
				{
					keyLayouts.Close();
				}
			}
			return list;
		}
	}
}
