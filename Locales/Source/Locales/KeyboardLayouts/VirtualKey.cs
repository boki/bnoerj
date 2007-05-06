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
	public class VirtualKey
	{
		VirtualKeyFlags flags;
		uint vk;
		uint sc;
		VirtualKeyValue[] shiftStateValues;

		public VirtualKey(VirtualKeyFlags flags, uint virtualKey, uint scanCode, VirtualKeyValue[] shiftStateValues)
		{
			this.flags = flags;
			this.vk = virtualKey;
			this.sc = scanCode;
			this.shiftStateValues = new VirtualKeyValue[shiftStateValues.Length];
			for (int i = 0; i < shiftStateValues.Length; i++)
			{
				this.shiftStateValues[i] = new VirtualKeyValue(scanCode, shiftStateValues[i]);
			}
		}

		public KeysEx VK
		{
			get { return (KeysEx)vk; }
		}

		public uint SC
		{
			get { return sc; }
		}

		public bool IsSGCaps
		{
			get { return (flags & VirtualKeyFlags.SGCaps) != 0; }
		}

		public bool IsCapsEqualToShift
		{
			get { return (flags & VirtualKeyFlags.CapsEqualToShift) != 0; }
		}

		public bool IsAltGrCapsEqualToAltGrShift
		{
			get { return (flags & VirtualKeyFlags.AltGrCapsEqualToAltGrShift) != 0; }
		}

		public bool IsSpecialGrCapsEqualToSpecialShift
		{
			get { return (flags & VirtualKeyFlags.SpecialGrCapsEqualToSpecialShift) != 0;  }
		}

		public bool IsEmpty
		{
			get { return (flags & VirtualKeyFlags.Empty) != 0; }
		}

		public VirtualKeyFlags Flags
		{
			get { return flags; }
		}

		public VirtualKeyValue[] ShiftStates
		{
			get { return shiftStateValues; }
		}

		public VirtualKeyValue GetShiftState(ShiftState shiftState, bool capsLock)
		{
			return shiftStateValues[(uint)shiftState * 2 + (capsLock ? 1 : 0)];
		}
	}
}
