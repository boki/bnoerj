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
	/// Defines a keyboard layouts virtual key value.
	/// </summary>
	public struct VirtualKeyValue
	{
		/// <summary>
		/// Represents a VirtualKeyValue that is a null reference.
		/// </summary>
		public static readonly VirtualKeyValue Empty = new VirtualKeyValue(uint.MaxValue, "", true);

		//NOTE: Creative alternative...
		internal readonly uint ScanCode;
		/// <summary>The characters for the scan code.</summary>
		public readonly String Characters;
		/// <summary>Indicator if this virtual key is a dead key or not.</summary>
		public readonly bool IsDeadKey;

		//FIXME: should be internal, but the Importer needs to construct this type
		public VirtualKeyValue(String characters, bool isDeadKey)
		{
			ScanCode = 0;
			Characters = characters;
			IsDeadKey = isDeadKey;
		}
		public VirtualKeyValue(uint scanCode, String characters, bool isDeadKey)
		{
			ScanCode = scanCode;
			Characters = characters;
			IsDeadKey = isDeadKey;
		}
		internal VirtualKeyValue(uint scanCode, VirtualKeyValue value)
			: this(scanCode, value.Characters, value.IsDeadKey)
		{
		}

		public static bool operator == (VirtualKeyValue value1, VirtualKeyValue value2)
		{
			return value1.Characters == value2.Characters && value1.IsDeadKey == value2.IsDeadKey;
		}

		public static bool operator !=(VirtualKeyValue value1, VirtualKeyValue value2)
		{
			return value1.Characters != value2.Characters || value1.IsDeadKey != value2.IsDeadKey;
		}

		public override bool Equals(object obj)
		{
			return this == (VirtualKeyValue)obj;
		}

		public override int GetHashCode()
		{
			return Characters.GetHashCode() ^ ScanCode.GetHashCode();
		}
	}
}
