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
	/// Defines a keyboard layouts virtual key.
	/// </summary>
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

		/// <summary>
		/// Gets the virtual keys code.
		/// </summary>
		public KeysEx VK
		{
			get { return (KeysEx)vk; }
		}

		/// <summary>
		/// Gets the scan code.
		/// </summary>
		public uint SC
		{
			get { return sc; }
		}

		/// <summary>
		/// Gets a value that indicates whether the virtual key defines a
		/// "Swiss German" caps state or not.
		/// </summary>
		public bool IsSGCaps
		{
			get { return (flags & VirtualKeyFlags.SGCaps) != 0; }
		}

		/// <summary>
		/// Gets a value indicating whether the virtual keys caps lock state
		/// is the same as its shift state or not.
		/// </summary>
		public bool IsCapsEqualToShift
		{
			get { return (flags & VirtualKeyFlags.CapsEqualToShift) != 0; }
		}

		/// <summary>
		/// Gets a value indicating whether the virtual keys AltGr caps lock
		/// state is the same as its AltGr shift state or not.
		/// </summary>
		public bool IsAltGrCapsEqualToAltGrShift
		{
			get { return (flags & VirtualKeyFlags.AltGrCapsEqualToAltGrShift) != 0; }
		}

		/// <summary>
		/// Gets a value indicating whether the virtual keys special shift
		/// caps lock state is the same as its special shift shift state or not.
		/// </summary>
		public bool IsSpecialGrCapsEqualToSpecialShift
		{
			get { return (flags & VirtualKeyFlags.SpecialGrCapsEqualToSpecialShift) != 0;  }
		}

		/// <summary>
		/// Gets a value indicating if the virtual key is empty or not.
		/// </summary>
		public bool IsEmpty
		{
			get { return (flags & VirtualKeyFlags.Empty) != 0; }
		}

		/// <summary>
		/// Gets the virtual keys flags.
		/// </summary>
		public VirtualKeyFlags Flags
		{
			get { return flags; }
		}

		/// <summary>
		/// Gets an array of values that correspond to the virtual keys shift
		/// states.
		/// </summary>
		public VirtualKeyValue[] ShiftStates
		{
			get { return shiftStateValues; }
		}

		/// <summary>
		/// Gets a virtual key value for a specified shift and caps lock
		/// state.
		/// </summary>
		/// <param name="shiftState">Enumerated value that specifies the shift state to use.</param>
		/// <param name="capsLock">Boolean value that specifies whether to use caps lock or not.</param>
		/// <returns>The virtual key value.</returns>
		public VirtualKeyValue GetShiftState(ShiftState shiftState, bool capsLock)
		{
			return shiftStateValues[(uint)shiftState * 2 + (capsLock ? 1 : 0)];
		}
	}
}
