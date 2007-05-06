// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Bnoerj.Locales.KeyboardLayouts;

namespace Bnoerj.Locales.Importer.KeyboardLayouts
{
	[ContentTypeWriter]
	class KeyboardLayoutWriter : ContentTypeWriter<KeyboardLayoutContent>
	{
		protected override void Write(ContentWriter output, KeyboardLayoutContent value)
		{
			output.Write(value.LayoutId);
			output.Write((uint)value.SpecialShiftVk);
			WriteVirtualKeys(output, value.Keys);
			WriteDeadKeys(output, value.DeadKeys);
		}

		/*public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return typeof(KeyboardLayout).AssemblyQualifiedName;
		}*/

		public override string GetRuntimeReader(Microsoft.Xna.Framework.TargetPlatform targetPlatform)
		{
			return typeof(KeyboardLayoutReader).AssemblyQualifiedName;
		}

		void WriteVirtualKeys(ContentWriter output, VirtualKeyContent[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				bool hasValue = keys[i] != null && keys[i].IsEmpty == false;
				output.Write(hasValue);
				if (hasValue == true)
				{
					WriteVirtualKey(output, keys[i].VirtualKey);
				}
			}
		}

		void WriteVirtualKey(ContentWriter output, VirtualKey key)
		{
			output.Write((uint)key.VK);
			output.Write(key.SC);
			output.Write((uint)key.Flags);
			foreach(VirtualKeyValue value in key.ShiftStates)
			{
				bool hasValue = value != VirtualKeyValue.Empty;
				output.Write(hasValue);
				if (hasValue == true)
				{
					WriteVirtualKeyValue(output, value);
				}
			}
		}

		void WriteVirtualKeyValue(ContentWriter output, VirtualKeyValue value)
		{
			output.Write(value.Characters);
			output.Write(value.IsDeadKey);
		}

		void WriteDeadKeys(ContentWriter output, Dictionary<char, DeadKey> keys)
		{
			output.Write(keys.Count);
			foreach (KeyValuePair<char, DeadKey> value in keys)
			{
				output.Write(value.Key);
				output.Write(value.Value.Count);
				foreach (char baseChar in value.Value.BaseCharacters)
				{
					output.Write(baseChar);
				}
				foreach (char combChar in value.Value.CombinedCharacters)
				{
					output.Write(combChar);
				}
			}
		}
	}
}
