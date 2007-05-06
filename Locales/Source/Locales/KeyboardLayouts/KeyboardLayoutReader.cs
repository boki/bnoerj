// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Bnoerj.Locales.KeyboardLayouts
{
	public class KeyboardLayoutReader : ContentTypeReader<KeyboardLayout>
	{
		protected override KeyboardLayout Read(ContentReader input, KeyboardLayout existingInstance)
		{
			String layoutId = input.ReadString();
			KeysEx specialShiftVk = (KeysEx)input.ReadUInt32();
			VirtualKey[] keys = ReadVirtualKeys(input);
			Dictionary<char, DeadKey> deadKeys = ReadDeadKeys(input);
			return new KeyboardLayout(layoutId, specialShiftVk, keys, deadKeys);
		}

		VirtualKey[] ReadVirtualKeys(ContentReader input)
		{
			VirtualKey[] keys = new VirtualKey[256];
			for (int i = 0; i < keys.Length; i++)
			{
				bool hasValue = input.ReadBoolean();
				if (hasValue == true)
				{
					keys[i] = ReadVirtualKey(input);
				}
			}
			return keys;
		}

		VirtualKey ReadVirtualKey(ContentReader input)
		{
			uint vk = input.ReadUInt32();
			uint sc = input.ReadUInt32();
			VirtualKeyFlags flags = (VirtualKeyFlags)input.ReadUInt32();
			VirtualKeyValue[] shiftStates = new VirtualKeyValue[((int)ShiftState.ShftSpcl + 1) * 2];
			for (int i = 0; i < shiftStates.Length; i++)
			{
				bool hasValue = input.ReadBoolean();
				if (hasValue == true)
				{
					shiftStates[i] = ReadVirtualKeyValue(input);
				}
			}
			return new VirtualKey(flags, vk, sc, shiftStates);
		}

		VirtualKeyValue ReadVirtualKeyValue(ContentReader input)
		{
			String chars = input.ReadString();
			bool isDeadKey = input.ReadBoolean();
			return new VirtualKeyValue(chars, isDeadKey);
		}

		Dictionary<char, DeadKey> ReadDeadKeys(ContentReader input)
		{
			int length = input.ReadInt32();
			Dictionary<char, DeadKey> deadKeys = new Dictionary<char, DeadKey>(length);
			for (int i = 0; i < length; i++)
			{
				ReadDeadKey(input, deadKeys);
			}
			return deadKeys;
		}

		void ReadDeadKey(ContentReader input, Dictionary<char, DeadKey> deadKeys)
		{
			char deadChar = input.ReadChar();
			int count = input.ReadInt32();
			char[] baseChars = input.ReadChars(count);
			char[] combChars = input.ReadChars(count);
			deadKeys.Add(deadChar, new DeadKey(deadChar, baseChars, combChars));
		}
	}
}
