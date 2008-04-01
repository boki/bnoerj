// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;

namespace Bnoerj.Audio.XapElements
{
	interface IElement
	{
		String Name { get; }
		Object Value { get; set; }
	}
}
