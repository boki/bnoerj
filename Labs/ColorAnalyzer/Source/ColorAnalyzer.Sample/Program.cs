// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Bnoerj.ColorAnalyzer
{
	using System;

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (Game1 game = new Game1())
			{
				game.Run();
			}
		}
	}
}

