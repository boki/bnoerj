// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Bnoerj.Locales.Importer.KeyboardLayouts.KlcFiles
{
	[ContentImporter(".klc", DisplayName = "Microsoft Keyboard Layout Creator - Bnoerj", DefaultProcessor = "KlcProcessor")]
	class KlcImporter : ContentImporter<KlcSource>
	{
		public override KlcSource Import(String filename, ContentImporterContext context)
		{
			String source = File.ReadAllText(filename);
			return new KlcSource(source);
		}
	}
}
