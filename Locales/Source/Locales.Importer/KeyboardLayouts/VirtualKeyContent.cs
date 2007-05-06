// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bnoerj.Locales.KeyboardLayouts;

namespace Bnoerj.Locales.Importer.KeyboardLayouts
{
	internal class VirtualKeyContent
	{
		uint vk;
		uint sc;
		VirtualKeyValue[] shiftStateValues;

		internal VirtualKeyContent(uint virtualKey, uint scanCode)
		{
			this.vk = virtualKey;
			this.sc = scanCode;
			this.shiftStateValues = new VirtualKeyValue[((int)ShiftState.ShftSpcl + 1) * 2];

			for (int i = 0; i < shiftStateValues.Length; i++)
			{
				shiftStateValues[i] = VirtualKeyValue.Empty;
			}
		}

		internal VirtualKeyContent(IntPtr hkl, KeysEx virtualKey)
			: this((uint)virtualKey, NativeMethods.User32.MapVirtualKeyEx((uint)virtualKey, 0, hkl))
		{
		}

		internal VirtualKeyContent(IntPtr hkl, uint scanCode)
			: this(NativeMethods.User32.MapVirtualKeyEx(scanCode, 1, hkl), scanCode)
		{
		}

		internal KeysEx VK
		{
			get { return (KeysEx)vk; }
		}

		internal uint SC
		{
			get { return sc; }
		}

		internal bool IsSGCaps
		{
			get
			{
				String stBase = GetShiftState(ShiftState.Base, false).Characters;
				String stShift = GetShiftState(ShiftState.Shft, false).Characters;
				String stCaps = GetShiftState(ShiftState.Base, true).Characters;
				String stShiftCaps = GetShiftState(ShiftState.Shft, true).Characters;
				return (stCaps.Length > 0 && !stBase.Equals(stCaps) && !stShift.Equals(stCaps)) ||
					(stShiftCaps.Length > 0 && !stBase.Equals(stShiftCaps) && !stShift.Equals(stShiftCaps));
			}
		}

		internal bool IsCapsEqualToShift
		{
			get
			{
				String stBase = GetShiftState(ShiftState.Base, false).Characters;
				String stShift = GetShiftState(ShiftState.Shft, false).Characters;
				String stCaps = GetShiftState(ShiftState.Base, true).Characters;
				return stBase.Length > 0 && stShift.Length > 0 && !stBase.Equals(stShift) && stShift.Equals(stCaps);
			}
		}

		internal bool IsAltGrCapsEqualToAltGrShift
		{
			get
			{
				String stBase = GetShiftState(ShiftState.MenuCtrl, false).Characters;
				String stShift = GetShiftState(ShiftState.ShftMenuCtrl, false).Characters;
				String stCaps = GetShiftState(ShiftState.MenuCtrl, true).Characters;
				return stBase.Length > 0 && stShift.Length > 0 && !stBase.Equals(stShift) && stShift.Equals(stCaps);
			}
		}

		internal bool IsSpecialGrCapsEqualToSpecialShift
		{
			get
			{
				String stBase = GetShiftState(ShiftState.Spcl, false).Characters;
				String stShift = GetShiftState(ShiftState.ShftSpcl, false).Characters;
				String stCaps = GetShiftState(ShiftState.Spcl, true).Characters;
				return stBase.Length > 0 && stShift.Length > 0 && !stBase.Equals(stShift) && stShift.Equals(stCaps);
			}
		}

		internal bool IsEmpty
		{
			get
			{
				for (int i = 0; i < shiftStateValues.Length; i++)
				{
					if (shiftStateValues[i] != VirtualKeyValue.Empty)
					{
						return false;
					}
				}
				return true;
			}
		}

		internal VirtualKeyValue[] ShiftStates
		{
			get { return shiftStateValues; }
		}

		internal VirtualKey VirtualKey
		{
			get
			{
				VirtualKeyFlags flags =
					(IsSGCaps ? VirtualKeyFlags.SGCaps : 0) |
					(IsCapsEqualToShift ? VirtualKeyFlags.CapsEqualToShift : 0) |
					(IsAltGrCapsEqualToAltGrShift ? VirtualKeyFlags.AltGrCapsEqualToAltGrShift : 0) |
					(IsSpecialGrCapsEqualToSpecialShift ? VirtualKeyFlags.SpecialGrCapsEqualToSpecialShift : 0) |
					(IsEmpty ? VirtualKeyFlags.Empty : 0);
				return new VirtualKey(flags, (uint)VK, SC, ShiftStates);
			}
		}

		internal VirtualKeyValue GetShiftState(ShiftState shiftState, bool capsLock)
		{
			return shiftStateValues[(uint)shiftState * 2 + (capsLock ? 1 : 0)];
		}

		internal void SetShiftState(ShiftState shiftState, String value, bool isDeadKey, bool capsLock)
		{
			shiftStateValues[(uint)shiftState * 2 + (capsLock ? 1 : 0)] = new VirtualKeyValue(value, isDeadKey);
		}

		internal String LayoutRow(KeyboardLayoutContent kbl)
		{
			StringBuilder sbRow = new StringBuilder();

			// First, get the SC/VK info stored
			sbRow.Append(String.Format("{0:x2}\t{1:x2} - {2}", SC, (byte)VK, ((KeysEx)VK).ToString().PadRight(13)));

			// Now the CAPSLOCK value
			int capslock =
				0 |
				(IsCapsEqualToShift ? 1 : 0) |
				(IsSGCaps ? 2 : 0) |
				(IsAltGrCapsEqualToAltGrShift ? 4 : 0) |
				(IsSpecialGrCapsEqualToSpecialShift ? 8 : 0);
			sbRow.Append(String.Format("\t{0}", capslock));


			for (ShiftState ss = 0; ss <= kbl.MaxShiftState; ss++)
			{
				if (ss == ShiftState.Menu || ss == ShiftState.ShftMenu)
				{
					// Alt and Shift+Alt ain't supported in keyboard layouts, so skip them
					continue;
				}

				for (int caps = 0; caps <= 1; caps++)
				{
					VirtualKeyValue vkv = GetShiftState(ss, (caps == 1));

					if (vkv.Characters.Length == 0)
					{
						// No character assigned here, put in -1.
						sbRow.Append("\t  -1");
					}
					else if ((caps == 1) && vkv == GetShiftState(ss, (caps == 0)))
					{
						// Its a CAPS LOCK state and the assigned character(s) are
						// identical to the non-CAPS LOCK state. Put in a MIDDLE DOT.
						sbRow.Append("\t   .");
						//sbRow.Append("\t   \u00b7");
					}
					else if (vkv.IsDeadKey == true)
					{
						// It's a dead key, append an @ sign.
						sbRow.Append(String.Format("\t{0:x4}@", ((ushort)vkv.Characters[0])));
					}
					else
					{
						// It's some characters; put 'em in there.
						StringBuilder sbChar = new StringBuilder((5 * vkv.Characters.Length) + 1);
						for (int ich = 0; ich < vkv.Characters.Length; ich++)
						{
							sbChar.Append(((ushort)vkv.Characters[ich]).ToString("x4"));
							sbChar.Append(' ');
						}
						sbRow.Append(String.Format("\t{0}", sbChar.ToString(0, sbChar.Length - 1)));
					}
				}
			}

			return sbRow.ToString();
		}
	}
}
