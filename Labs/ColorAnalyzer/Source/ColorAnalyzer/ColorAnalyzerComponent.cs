// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Bnoerj.ColorAnalyzer
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Content;
	using Microsoft.Xna.Framework.Graphics;
	using Microsoft.Xna.Framework.Input;

	public class ColorAnalyzerComponent : DrawableGameComponent
	{
		enum ColorView
		{
			ColorBrightness,
			Protanopia,
			Deutanopia,
			Tritanopia,
			TypicalAchromatopsia,
			Protanomaly,
			Deutanomaly,
			Tritanomaly,
			AtypicalAchromatopsia,
			MaxViews
		}

		const int Padding = 4;

		ContentManager content;

		SpriteBatch spriteBatch;
		SpriteFont font;

		Texture2D blankTexture;
		ResolveTexture2D resolveTarget;

		Effect effect;
		EffectParameter paramTexture;

		VertexPositionTexture[] vertices;
		VertexDeclaration vertexDeclaration;
		VertexBuffer vertexBuffer;

		GamePadState lastGamePadState;
		KeyboardState lastKeyboardState;

		bool showUi;
		ColorView currentView;
		string currentViewText;

		public ColorAnalyzerComponent(Game game)
			: base(game)
		{
			Visible = false;
			DrawOrder = 0x7FFFFFFF;

			lastGamePadState = new GamePadState();
			lastKeyboardState = new KeyboardState();

			showUi = true;
		}

		public PlayerIndex ControllingPlayer
		{
			get;
			set;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			if (content == null)
			{
				content = new ContentManager(Game.Services, "AnalyzerContent");

				spriteBatch = new SpriteBatch(GraphicsDevice);

				font = content.Load<SpriteFont>("Font");

				blankTexture = new Texture2D(GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
				var colors = new Color[]
				{
					Color.White
				};
				blankTexture.SetData<Color>(colors);

				var presentationParams = GraphicsDevice.PresentationParameters;
				var width = presentationParams.BackBufferWidth;
				var height = presentationParams.BackBufferHeight;
				resolveTarget = new ResolveTexture2D(GraphicsDevice,
					width, height, 1,
					presentationParams.BackBufferFormat);

				effect = content.Load<Effect>("Shaders/ColorView");
				paramTexture = effect.Parameters["Texture"];

				effect.Parameters["ViewportSize"].SetValue(new Vector2(width, height));

				var device = GraphicsDevice;
				vertices = new VertexPositionTexture[4]
				{
					new VertexPositionTexture(
						new Vector3(width, height, 0.0f),
						new Vector2(1.0f, 1.0f)),
					new VertexPositionTexture(
						new Vector3(0.0f, height, 0.0f),
						new Vector2(0.0f, 1.0f)),
					new VertexPositionTexture(
						new Vector3(0.0f, 0.0f, 0.0f),
						new Vector2(0.0f, 0.0f)),
					new VertexPositionTexture(
						new Vector3(width, 0.0f, 0.0f),
						new Vector2(1.0f, 0.0f)),
				};

				vertexDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);
				vertexBuffer = new VertexBuffer(device, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);

				ChangeView(0);
			}
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();

			if (content != null)
			{
				content.Unload();
				content.Dispose();
				content = null;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (Visible == true)
			{
				var keyboardState = Keyboard.GetState();
				var gamePadState = GamePad.GetState(ControllingPlayer);

				if (IsNewKeyPress(keyboardState, Keys.F1) == true ||
					IsNewButtonPress(gamePadState, Buttons.B) == true)
				{
					showUi = !showUi;
				}
				else if (IsNewKeyPress(keyboardState, Keys.Left) == true ||
					IsNewButtonPress(gamePadState, Buttons.DPadRight) == true ||
					IsNewButtonPress(gamePadState, Buttons.A) == true)
				{
					ChangeView((int)currentView + 1);
				}
				else if (IsNewKeyPress(keyboardState, Keys.Right) == true ||
					IsNewButtonPress(gamePadState, Buttons.DPadLeft) == true)
				{
					ChangeView((int)currentView - 1);
				}

				lastKeyboardState = keyboardState;
				lastGamePadState = gamePadState;
			}
		}

		public void BeginDraw()
		{
		}

		public void EndDraw()
		{
			if (Visible == false)
			{
				return;
			}

			GraphicsDevice.ResolveBackBuffer(resolveTarget);

			GraphicsDevice.VertexDeclaration = vertexDeclaration;

			paramTexture.SetValue(resolveTarget);

			effect.Begin();

			var pass = effect.CurrentTechnique.Passes[0];
			pass.Begin();

			GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleFan,
				vertices, 0, 2);

			pass.End();
			effect.End();

			if (showUi == true)
			{
				DrawUI();
			}
		}

		void DrawUI()
		{
			var rcFull = GraphicsDevice.Viewport.TitleSafeArea;

			var rc = new Rectangle()
			{
				//X = rcFull.Right - rcFull.Width / 4 - 2 * Padding,
				//Y = rcFull.Bottom - (int)(8 * font.LineSpacing) - 2 * Padding,
				X = 0,
				Y = 0,
				Width = rcFull.Width / 3 + 2 * Padding,
				Height = (int)(3 * font.LineSpacing) + 2 * Padding
			};

			var position = new Vector2()
			{
				X = rc.X + Padding,
				Y = rc.Y + Padding
			};

			spriteBatch.Begin();

			spriteBatch.Draw(blankTexture, rc, Color.Black);

			spriteBatch.DrawString(font, currentViewText, position, Color.Yellow);

			position.Y += font.LineSpacing;
			spriteBatch.DrawString(font, "Toggle View: A, DPad left/right", position, Color.Yellow);

			position.Y += font.LineSpacing;
			spriteBatch.DrawString(font, "Toggle Help: B", position, Color.Yellow);

			spriteBatch.End();
		}

		void ChangeView(int newViewIndex)
		{
			if (newViewIndex >= (int)ColorView.MaxViews)
			{
				newViewIndex = 0;
			}
			else if (newViewIndex < 0)
			{
				newViewIndex = (int)ColorView.MaxViews - 1;
			}

			currentView = (ColorView)newViewIndex;

			currentViewText = string.Format("View: {0}", currentView);

			effect.CurrentTechnique = effect.Techniques[newViewIndex];
		}

		bool IsNewKeyPress(KeyboardState keyboardState, Keys key)
		{
			return keyboardState.IsKeyDown(key) == true &&
				lastKeyboardState.IsKeyUp(key) == true;
		}

		bool IsNewButtonPress(GamePadState gamePadState, Buttons button)
		{
			return gamePadState.IsButtonDown(button) == true &&
				lastGamePadState.IsButtonUp(button) == true;
		}
	}
}
