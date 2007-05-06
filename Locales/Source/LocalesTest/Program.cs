// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;

namespace LocaleTest
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
#if DUMP_SCANCODES
			Bnoerj.Locales.Importer.Temp.TestImport(0x0409);
			//Bnoerj.Locales.Importer.Temp.DumpScanCodesVirtualKeys("00000409");
#if HIDE_KLCFILE_IMPORTER
			Bnoerj.Locales.Importer.KlcImporter i = new Bnoerj.Locales.Importer.KlcImporter();
			i.Import(@"C:\Dokumente und Einstellungen\Boki\Eigene Dateien\Keyboard Layouts\bg-02.klc", null);
#endif
#else
			using (Game1 game = new Game1())
			{
				game.Run();
			}
#endif
		}
	}
}

