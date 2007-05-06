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
	public class VirtualKeyReader : ContentTypeReader<VirtualKey>
	{
		protected override VirtualKey Read(ContentReader input, VirtualKey existingInstance)
		{
			uint vk = input.ReadUInt32();
			uint sc = input.ReadUInt32();
			VirtualKeyFlags flags = (VirtualKeyFlags)input.ReadUInt32();
			VirtualKeyValue[] shiftStates = input.ReadObject<VirtualKeyValue[]>();
			return new VirtualKey(flags, vk, sc, shiftStates);
		}
	}
}
