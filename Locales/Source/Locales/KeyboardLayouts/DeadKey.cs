// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bnoerj.Locales.KeyboardLayouts
{
	public class DeadKey
	{
		char deadCharacter;
		Dictionary<char, char> baseCombinedCharacters;

		public DeadKey(char deadCharacter)
		{
			this.deadCharacter = deadCharacter;
			this.baseCombinedCharacters = new Dictionary<char, char>();
		}

		public DeadKey(char deadCharacter, char[] baseChars, char[] combChars)
		{
			this.deadCharacter = deadCharacter;
			this.baseCombinedCharacters = new Dictionary<char, char>();
			for (int i = 0; i < baseChars.Length; i++)
			{
				this.baseCombinedCharacters.Add(baseChars[i], combChars[i]);
			}
		}

		public char DeadCharacter
		{
			get { return deadCharacter; }
		}

		public int Count
		{
			get { return baseCombinedCharacters.Count; }
		}

		public Dictionary<char, char>.KeyCollection BaseCharacters
		{
			get { return baseCombinedCharacters.Keys; }
		}

		public Dictionary<char, char>.ValueCollection CombinedCharacters
		{
			get { return baseCombinedCharacters.Values; }
		}

		public void AddDeadKeyRow(char baseCharacter, char combinedCharacter)
		{
			baseCombinedCharacters.Add(baseCharacter, combinedCharacter);
		}

		public bool ContainsBaseCharacter(char baseCharacter)
		{
			return baseCombinedCharacters.ContainsKey(baseCharacter);
		}

		public char GetCombinedCharacter(char baseCharacter)
		{
			return baseCombinedCharacters[baseCharacter];
		}
	}
}
