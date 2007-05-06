// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Bnoerj.Locales.Importer.KeyboardLayouts.KlcFiles
{
	[ContentProcessor(DisplayName = "KLC - Bnoerj")]
	class KlcProcessor : ContentProcessor<KlcSource, KeyboardLayoutContent>
	{
		public override KeyboardLayoutContent Process(KlcSource input, ContentProcessorContext context)
		{
			return null;// new KeyboardLayoutContent();
		}
	}
}
