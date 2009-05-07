// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Bnoerj.ColorAnalyzer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Audio;
	using Microsoft.Xna.Framework.Content;
	using Microsoft.Xna.Framework.GamerServices;
	using Microsoft.Xna.Framework.Graphics;
	using Microsoft.Xna.Framework.Input;
	using Microsoft.Xna.Framework.Media;
	using Microsoft.Xna.Framework.Net;
	using Microsoft.Xna.Framework.Storage;

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		const float Padding = 8;

		GraphicsDeviceManager graphics;

		SpriteBatch spriteBatch;
		SpriteFont font;

		Texture2D blankTexture;

		ColorAnalyzerComponent colorAnalyzer;

		float textBoxWidth;

		GamePadState lastGamePadState;
		KeyboardState lastKeyboardState;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
#if XBOX
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
#else
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 576;
#endif
			Content.RootDirectory = "Content";

			colorAnalyzer = new ColorAnalyzerComponent(this);
			Components.Add(colorAnalyzer);

			lastGamePadState = new GamePadState();
			lastKeyboardState = new KeyboardState();
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

			blankTexture = new Texture2D(GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
			var colors = new Color[]
			{
				Color.White
			};
			blankTexture.SetData<Color>(colors);

			var viewport = GraphicsDevice.Viewport.TitleSafeArea;
			textBoxWidth = 0.5f * viewport.Width;
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
			var keyboardState = Keyboard.GetState();
			var gamePadState = GamePad.GetState(PlayerIndex.One);

			// Allows the game to exit
			if (keyboardState.IsKeyDown(Keys.Escape) == true ||
				GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				Exit();
			}

			if ((keyboardState.IsKeyDown(Keys.F12) == true &&
				lastKeyboardState.IsKeyUp(Keys.F12) == true) ||
				(gamePadState.IsButtonDown(Buttons.Start) == true &&
				lastGamePadState.IsButtonUp(Buttons.Start) == true))
			{
				colorAnalyzer.ControllingPlayer = PlayerIndex.One;
				colorAnalyzer.Visible = !colorAnalyzer.Visible;
			}

			lastKeyboardState = keyboardState;
			lastGamePadState = gamePadState;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			var viewport = GraphicsDevice.Viewport.TitleSafeArea;
			var position = new Vector2()
			{
				X = viewport.X + (int)(0.5f * (viewport.Width - textBoxWidth)),
				Y = viewport.Y
			};

			spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

			DrawText("High Contrast", ref position, Color.Yellow, Color.Black);

			DrawText("Low Contrast", ref position,
				new Color(0xFF, 0x00, 0x00),
				new Color(0x00, 0x00, 0xFF));

			DrawText("Low Contrast", ref position,
				new Color(0xFF, 0xFF, 0xFF),
				new Color(0xCC, 0xCC, 0xCC));

			DrawText("Color Blind: Protanopia", ref position,
				new Color(0x00, 0xFF, 0x80),
				new Color(0xFF, 0x80, 0x00));
			DrawText("Color Blind: Deuteranopia", ref position,
				new Color(0xFF, 0x80, 0x00),
				new Color(0xFF, 0xFF, 0x00));
			DrawText("Color Blind: Tritanopia", ref position,
				new Color(0x80, 0xFF, 0x80),
				new Color(0x80, 0x80, 0xFF));

			DrawText("NTSC Clipped", ref position,
				new Color(0xFF, 0xFF, 0xFF),
				new Color(0x00, 0x00, 0x00));

			DrawText("Press Start", ref position, Color.Yellow, Color.Black);

			spriteBatch.End();

			base.Draw(gameTime);
		}

		protected override bool BeginDraw()
		{
			bool result = base.BeginDraw();
			if (result == true)
			{
				colorAnalyzer.BeginDraw();
			}

			return result;
		}

		protected override void EndDraw()
		{
			colorAnalyzer.EndDraw();

			base.EndDraw();
		}

		void DrawText(string text, ref Vector2 position, Color foregroundColor, Color backgroundColor)
		{
			var rc = new Rectangle()
			{
				X = (int)position.X,
				Y = (int)position.Y,
				Width = (int)textBoxWidth,
				Height = (int)(font.LineSpacing + 2 * Padding)
			};
			spriteBatch.Draw(blankTexture, rc, backgroundColor);

			var pos = position;
			pos.X += Padding;
			pos.Y += Padding;
			spriteBatch.DrawString(font, text, pos, foregroundColor);

			position.Y += rc.Height + Padding;
		}
	}
}
