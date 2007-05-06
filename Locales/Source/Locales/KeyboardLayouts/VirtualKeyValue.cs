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
	public struct VirtualKeyValue
	{
		public static readonly VirtualKeyValue Empty = new VirtualKeyValue("", true);

		//NOTE: Creative alternative...
		internal readonly uint ScanCode;
		public readonly String Characters;
		public readonly bool IsDeadKey;

		//FIXME: should be internal, but the Importer needs to construct this type
		public VirtualKeyValue(String characters, bool isDeadKey)
		{
			ScanCode = 0;
			Characters = characters;
			IsDeadKey = isDeadKey;
		}
		internal VirtualKeyValue(uint scanCode, VirtualKeyValue value)
		{
			ScanCode = scanCode;
			Characters = value.Characters;
			IsDeadKey = value.IsDeadKey;
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
			//FIXME: what about overflows?
			return Characters.GetHashCode() | GetHashCode();
		}
	}
}
