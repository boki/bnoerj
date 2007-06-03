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
using Microsoft.Xna.Framework.Input;
using Bnoerj.Locales.KeyboardLayouts;

namespace Bnoerj.Locales.Text
{
	/// <summary>
	/// The TextInputService ...
	/// </summary>
	public class TextInputService : GameComponent
	{
		KeyboardLayout keyboardLayout;

		int delay;
		int repeatSpeed;
		double delayTime;
		double repeatTime;

		KeyStateFlags[] keyStates;
		Keys[] previousStroke;
		VirtualKeyValue currentKeyValue;
		double strokeStartTime;
		DeadKey deadKey;
		int repeatTimeCount;

		String characters;

		/// <summary>
		/// Initializes a new instance of TextInputService.
		/// </summary>
		/// <param name="game">Game the TextInputService should be attached to.</param>
		public TextInputService(Game game)
			: base(game)
		{
			keyStates = new KeyStateFlags[(int)KeysEx.Last];

			Delay = 2;
			RepeatSpeed = 15;

			game.Services.AddService(typeof(TextInputService), this);
			game.Components.Add(this);
		}

		/// <summary>
		/// Gets or sets the keyboard repeat-delay, from 0 (approximately 250
		/// millisecond delay) through 3 (approximately 1 second delay).
		/// </summary>
		/// <remarks>
		/// This property indicates the amount of time that elapses after a
		/// key is pressed and held down until keystroke repeat messages are
		/// processed by the service.
		/// </remarks>
		public int Delay
		{
			get { return delay; }
			set
			{
				//FIXME: check for Game.IsFixedTimeStep/TargetElapsedTime?
				delay = Math.Max(0, Math.Min(value, 3));
				delayTime = (double)(250 + 250 * delay);
			}
		}

		/// <summary>
		/// Gets or sets the repeat-speed, from 0 (approximately 2.5 repetitions
		/// per second) through 31 (approximately 30 repetitions per second).
		/// </summary>
		/// <remarks>
		/// This property indicates the time between each keystroke repeat
		/// processing that is sent when a user presses and holds a key down.
		/// </remarks>
		public int RepeatSpeed
		{
			get { return repeatSpeed; }
			set
			{
				//FIXME: check for Game.IsFixedTimeStep/TargetElapsedTime?
				repeatSpeed = Math.Max(0, Math.Min(value, 31));
				// Repeat rate per second = (27.5/31 * repeatRate + 2.5) per second
				double repeatsPerSec = 2.5 + (27.5 / 31.0 * (double)repeatSpeed);
				repeatTime = 1000 / repeatsPerSec;
			}
		}

		/// <summary>
		/// Gets the characters for the current keystroke
		/// </summary>
		public String KeystrokeCharacters
		{
			get { return characters; }
		}

		public override void Initialize()
		{
			base.Initialize();

			LocaleService localeService = (LocaleService)Game.Services.GetService(typeof(LocaleService));
			keyboardLayout = localeService.CurrentLocale.KeyboardLayout;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			// Get current keystroke and check if this is a new keystroke or
			// if it is a repeated one.
			KeyboardState state = Keyboard.GetState();
			VirtualKeyValue[] keystroke = keyboardLayout.ProcessKeys(state);
			Keys[] filteredStroke = keyboardLayout.FilteredPressedKeys;
			bool isNewStroke = true;
			if (previousStroke != null)
			{
				// Check scan codes to handle modifier+char to char transitions.
				// If two or more keys are pressed, the last one is used and when
				// this key is released, no keystroke is generated.

				int newKeyIndex = -1;
				int i;
				int j = 0;
				for (i = 0; i < filteredStroke.Length; i++)
				{
					// KeyboardLayout.FilteredPressedKeys is sorted
					while (j < previousStroke.Length && previousStroke[j] < filteredStroke[i])
					{
						j++;
					}

					if (j >= previousStroke.Length)
					{
						newKeyIndex = i;
						break;
					}
					else if (previousStroke[j] > filteredStroke[i])
					{
						newKeyIndex = i;
					}
				}

				if (newKeyIndex > -1)
				{
					isNewStroke = currentKeyValue != keystroke[newKeyIndex];
					currentKeyValue = keystroke[newKeyIndex];
				}
				else
				{
					isNewStroke = false;
					if (keystroke.Length == 0)
					{
						currentKeyValue = VirtualKeyValue.Empty;
					}
				}
			}

			for (int i = 0; i < keyStates.Length; i++)
			{
				keyStates[i] = KeyStateFlags.Up;
			}

			// If the keystroke is a new stroke, reset start time and update
			// key state flags to notify that the key was just released
			if (isNewStroke == true)
			{
				strokeStartTime = gameTime.TotalGameTime.TotalMilliseconds;
				repeatTimeCount = -1;

				if (previousStroke != null)
				{
					foreach (Keys key in previousStroke)
					{
						keyStates[(int)key] = KeyStateFlags.Released;
					}
				}
			}

			// Apply delay and repeat-speed to stroke age
			double dt = gameTime.TotalGameTime.TotalMilliseconds - strokeStartTime;
			bool doSetString = false;
			KeyStateFlags newKeyStateFlags = KeyStateFlags.Down;
			if (dt <= float.Epsilon)
			{
				// First stroke
				doSetString = true;
			}
			else if (repeatTimeCount == -1 && delayTime - dt < float.Epsilon)
			{
				// Repeat delay time reached
				doSetString = true;
				repeatTimeCount = 0;

				newKeyStateFlags |= KeyStateFlags.Repeat;
			}
			else if (repeatTimeCount > -1)
			{
				dt -= delayTime;

				// Count stroke repeats
				int i;
				for (i = 0; dt > 0; i++)
				{
					dt -= repeatTime;
				}

				if (i > repeatTimeCount)
				{
					doSetString = true;
					repeatTimeCount++;
				}

				newKeyStateFlags |= KeyStateFlags.Repeat;
			}

			// Build the current key strokes output
			characters = "";
			if (doSetString == true && keystroke.Length > 0)
			{
				VirtualKeyValue keyValue = currentKeyValue;
				if (deadKey != null)
				{
					char baseChar = keyValue.Characters[0];
					if (deadKey.ContainsBaseCharacter(baseChar) == true)
					{
						characters = deadKey.GetCombinedCharacter(baseChar).ToString();
					}
					else
					{
						characters = String.Format("{0}{1}", deadKey.DeadCharacter, baseChar);
					}
					deadKey = null;
				}
				else if (keyValue.IsDeadKey == true)
				{
					keyboardLayout.DeadKeys.TryGetValue(keyValue.Characters[0], out deadKey);
				}
				else if (keyValue.Characters != null && Char.IsControl(keyValue.Characters, 0) == false)
				{
					// Add non-control characters only
					characters = keyValue.Characters;
				}
			}

			if (doSetString == true && filteredStroke.Length > 0)
			{
				// Update key state flags
				foreach (Keys key in filteredStroke)
				{
					keyStates[(int)key] = newKeyStateFlags;
				}
			}

			previousStroke = filteredStroke;
		}

		/// <summary>
		/// Gets the KeyStateFlags for the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public KeyStateFlags GetKeyStateFlag(KeysEx key)
		{
			return keyStates[(int)key];
		}

		/// <summary>
		/// Returns whether the specified key is currently being pressed.
		/// </summary>
		/// <param name="key">Enumerated value that specifies the key to query.</param>
		/// <returns>true if the key specified by key is being held down; false otherwise.</returns>
		public bool IsKeyDown(KeysEx key)
		{
			return (keyStates[(int)key] & KeyStateFlags.Down) != 0;
		}

		/// <summary>
		/// Returns whether the specified key is currently being pressed and
		/// if the press is repeated.
		/// </summary>
		/// <param name="key">Enumerated value that specifies the key to query.</param>
		/// <returns>true if the key specified by key is being held down and the press is repeated; false otherwise.</returns>
		public bool IsKeyRepeated(KeysEx key)
		{
			return (keyStates[(int)key] & KeyStateFlags.Repeat) != 0;
		}

		/// <summary>
		/// Returns whether the specified key has just been released.
		/// </summary>
		/// <param name="key">Enumerated value that specifies the key to query.</param>
		/// <returns>true if the key specified by key is being released; false otherwise.</returns>
		public bool IsKeyReleased(KeysEx key)
		{
			return (keyStates[(int)key] & KeyStateFlags.Released) != 0;
		}
	}
}
