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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Bnoerj.Locales;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Text;

namespace LocaleTest
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		ContentManager content;
		GlyphBatch glyphBatch;

		String[] localeNames;
		int curLocalNameIndex;
		LocaleService localeService;
		TextInputService textInputService;
		String inputText;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 960;
			graphics.PreferredBackBufferHeight = 600;

			curLocalNameIndex = -1;
			localeNames = new String[] { "en-US", "de-DE", "ru-RU" };

			content = new ContentManager(Services);
			localeService = new LocaleService(this, content);
			textInputService = new TextInputService(this);

			inputText = "";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		/// <summary>
		/// Called when graphics resources need to be loaded. Override this
		/// method to load any game-specific graphics resources.
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();

			glyphBatch = new GlyphBatch(graphics.GraphicsDevice);
		}

		/// <summary>
		/// Called when graphics resources need to be unloaded. Override this
		/// method to unload any game-specific graphics resources.
		/// </summary>
		protected override void UnloadContent()
		{
			base.UnloadContent();

			content.Unload();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			// Allows the default game to exit on Xbox 360 and Windows
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
			{
				this.Exit();
			}

			if (textInputService.IsKeyReleased(KeysEx.VK_TAB) == true)
			{
				//FIXME: Change current locale
				curLocalNameIndex = (curLocalNameIndex + 1) % localeNames.Length;
				localeService.LoadLocale(localeNames[curLocalNameIndex]);
				//FIXME: Move elsewhere, e.g. use an event
				textInputService.Initialize();
				inputText = "";
			}
			else if (textInputService.IsKeyDown(KeysEx.VK_BACK) == true && inputText.Length > 0)
			{
				inputText = inputText.Substring(0, inputText.Length - 1);
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			Font font = localeService.CurrentLocale.Font;
			float fontHeight = font.Height;
			glyphBatch.Begin(font);

			Vector2 pos = new Vector2(fontHeight, fontHeight);
			String text = textInputService.KeystrokeCharacters;
			if (text != null && text.Length > 0)
			{
				inputText += text;
				TextRenderer.DrawText(glyphBatch, text, font, pos, Color.Yellow);
			}

			TextRectangle bounds = new TextRectangle(fontHeight, fontHeight, graphics.GraphicsDevice.Viewport.Width - 2 * fontHeight, fontHeight);
			text = String.Format("Locale: {0} (Press TAB to change)", localeService.CurrentLocale.CultureInfo.Name);
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.White, TextFormatFlags.Right);

			bounds.Y += fontHeight;
			TextRenderer.DrawText(glyphBatch, inputText, font, bounds, Color.White);

			bounds.Y += fontHeight;
			text = String.Format("Delay: {0}\nRepeat-speed: {1}", textInputService.Delay.ToString(), textInputService.RepeatSpeed.ToString());
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.White);

			bounds.Y += 2 * fontHeight;
			text = String.Format("Black on yellow");
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.Black, Color.Yellow);

			bounds.Y += 1.5f * fontHeight;
			text = String.Format("horizontal centered");
			glyphBatch.DrawBackground(bounds.Position, bounds.Size, Color.Yellow);
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.Black, TextFormatFlags.HorizontalCenter);

			bounds.Y += 1.5f * fontHeight;
			bounds.Height = 2 * fontHeight;
			text = String.Format("vertical centered");
			glyphBatch.DrawBackground(bounds.Position, bounds.Size, Color.Yellow);
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);

			bounds.Y += 0.5f * fontHeight + bounds.Height;
			text = String.Format("horizontal and vertical centered");
			glyphBatch.DrawBackground(bounds.Position, bounds.Size, Color.Yellow);
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);

			bounds.Y += 0.5f * fontHeight + bounds.Height;
			text = String.Format("right and top aligned");
			glyphBatch.DrawBackground(bounds.Position, bounds.Size, Color.Yellow);
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.Black, TextFormatFlags.Right | TextFormatFlags.SingleLine);

			bounds.Y += 0.5f * fontHeight + bounds.Height;
			text = String.Format("left and bottom aligned");
			glyphBatch.DrawBackground(bounds.Position, bounds.Size, Color.Yellow);
			TextRenderer.DrawText(glyphBatch, text, font, bounds, Color.Black, TextFormatFlags.Bottom | TextFormatFlags.SingleLine);

			glyphBatch.End();

			base.Draw(gameTime);
		}
	}
}
