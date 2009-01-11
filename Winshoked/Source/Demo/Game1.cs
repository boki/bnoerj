// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Bnoerj.Winshoked;

namespace Demo
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont font;

		WinshokedComponent winshoked;

		KeyboardState lastKeyboardState;

		public Game1()
		{
			Window.Title = "Winshoked Demo";

			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			winshoked = new WinshokedComponent(this);
			Components.Add(winshoked);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			lastKeyboardState = new KeyboardState();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			font = Content.Load<SpriteFont>("Font");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			KeyboardState keyboardState = Keyboard.GetState();

			// Allows the game to exit
			if (keyboardState.IsKeyDown(Keys.Escape) == true ||
				GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				this.Exit();
			}

			if (keyboardState.IsKeyDown(Keys.Tab) == true &&
				lastKeyboardState.IsKeyDown(Keys.Tab) == false)
			{
				winshoked.Enabled = !winshoked.Enabled;
			}
			else if (keyboardState.IsKeyDown(Keys.X) == true)
			{
				throw new Exception("Testing cleanup code.");
			}

			lastKeyboardState = keyboardState;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			Vector2 pos = new Vector2(14, 14);

			spriteBatch.Begin();

			StringBuilder msg = new StringBuilder();
			msg.AppendFormat("Short cut keys are {0}",
				IsActive == true && winshoked.Enabled == true ? "disabled" : "enabled");
			spriteBatch.DrawString(font, msg, pos, Color.White);

			if (IsActive == true)
			{
				pos.Y += 21;
				msg = new StringBuilder();
				msg.AppendFormat("Press TAB to {0}",
					winshoked.Enabled ? "enable" : "disable");
				spriteBatch.DrawString(font, msg, pos, Color.White);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
