// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna.Sample
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Content;
	using Microsoft.Xna.Framework.Graphics;
	using Microsoft.Xna.Framework.Input;
	using Agg.Xna;
	using System.Diagnostics;

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class SampleGame : Microsoft.Xna.Framework.Game
	{
		struct Polygon
		{
			public Color Color;
			public double[] Points;
		}

		Random rng;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont font;

		Polygon[] polygons;

		byte[] alphaBuffer;
		byte[] colorBuffer;
		RenderingBuffer alphaRenderingBuffer;
		RenderingBuffer colorRenderingBuffer;
		Renderer alphaRenderer;
		Renderer colorRenderer;
		Rasterizer rasterizer;

		Texture2D textureAlpha;
		Texture2D textureColor;

		int frameCounter;
		double frameTime;
		float fps;

		Stopwatch stopwatch;
		String times;

		public SampleGame()
		{
			rng = new Random(1234);

			graphics = new GraphicsDeviceManager(this);
#if XBOX
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
#else
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 576;
#endif
			Content.RootDirectory = "Content";

			frameCounter = 0;
			frameTime = 0;
			fps = 0;

			stopwatch = new Stopwatch();
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
		protected unsafe override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont>("Font");

			textureAlpha = new Texture2D(GraphicsDevice, 512, 512, 1,
				TextureUsage.None, SurfaceFormat.Alpha8);
			textureColor = new Texture2D(GraphicsDevice, 512, 512, 1,
				TextureUsage.None, SurfaceFormat.Color);

			alphaBuffer = new byte[512 * 512];
			colorBuffer = new byte[512 * 512 * sizeof(Color)];

			alphaRenderingBuffer = new RenderingBuffer(alphaBuffer, 512, 512, 512);
			colorRenderingBuffer = new RenderingBuffer(colorBuffer, 512, 512, 512 * sizeof(Color));

			alphaRenderer = new Renderer(alphaRenderingBuffer, new SpanAlpha8());
			colorRenderer = new Renderer(colorRenderingBuffer, new SpanBgra32());

			rasterizer = new Rasterizer();
			rasterizer.SetGamma(1.3);
			rasterizer.FillRule = FillingRule.FillEvenOdd;

			CreatePolygons(512, 512);

			stopwatch.Start();
			Draw(alphaRenderer, alphaRenderingBuffer);
			long at = stopwatch.ElapsedMilliseconds;

			stopwatch.Reset();
			stopwatch.Start();
			Draw(colorRenderer, colorRenderingBuffer);
			long ct = stopwatch.ElapsedMilliseconds;

			times = String.Format("{0}ms ({1:0.000}), {2}ms ({3:0.000})",
				at, 1.0 / (double)at, ct, 1.0 / (double)ct);

			textureAlpha.SetData<byte>(alphaBuffer);
			textureColor.SetData<byte>(colorBuffer);

			textureAlpha.Save("alpha.png", ImageFileFormat.Png);
			textureColor.Save("color.png", ImageFileFormat.Png);
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
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
			{
				Exit();
			}

			frameCounter++;
			frameTime += gameTime.ElapsedGameTime.TotalSeconds;
			if (frameTime > 1.0)
			{
				fps = (float)(frameCounter / frameTime);

				frameTime -= 1.0;
				frameCounter = 0;

				//CreatePolygons(512, 512);

				stopwatch.Reset();
				stopwatch.Start();
				Draw(alphaRenderer, alphaRenderingBuffer);
				long at = stopwatch.ElapsedMilliseconds;

				stopwatch.Reset();
				stopwatch.Start();
				Draw(colorRenderer, colorRenderingBuffer);
				long ct = stopwatch.ElapsedMilliseconds;

				times = String.Format("{0}ms ({1:0.000}), {2}ms ({3:0.000})",
					at, 1.0 / (double)at, ct, 1.0 / (double)ct);

				textureAlpha.SetData<byte>(alphaBuffer);
				textureColor.SetData<byte>(colorBuffer);
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			Vector2 pos = Vector2.Zero;

			spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

			spriteBatch.Draw(textureAlpha, pos, Color.White);
			pos.X += textureAlpha.Width;
			spriteBatch.Draw(textureColor, pos, Color.White);

			pos.X = 52;
			pos.Y = 29;
			spriteBatch.DrawString(font, fps.ToString("0.0"), pos, Color.Yellow);
			pos.Y += font.LineSpacing;
			spriteBatch.DrawString(font, times, pos, Color.Yellow);

			spriteBatch.End();

			base.Draw(gameTime);
		}

		void CreatePolygons(int width, int height)
		{
			polygons = new Polygon[10];

			int i;

			// Draw random polygons
			for (i = 0; i < polygons.Length; i++)
			{
				int n = 3 + (rng.Next() % 6);

				polygons[i] = new Polygon();
				polygons[i].Points = new double[2 * n];

				// Make the polygon. One can call MoveTo() more than once. 
				// In this case the rasterizer behaves like Win32 API PolyPolygon().
				polygons[i].Points[0] = Random(-30, width + 30);
				polygons[i].Points[1] = Random(-30, height + 30);

				for (int j = 1; j < n; j++)
				{
					polygons[i].Points[2 * j + 0] = Random(-30, width + 30);
					polygons[i].Points[2 * j + 1] = Random(-30, height + 30);
				}

				// Render
				Color color = new Color();
				color.R = (byte)(rng.Next() & 0xFF);
				color.G = (byte)(rng.Next() & 0xFF);
				color.B = (byte)(rng.Next() & 0xFF);
				color.A = (byte)((rng.Next() & 0x7F) + 0x80);
				polygons[i].Color = color;
			}
		}

		void Draw(Renderer renderer, RenderingBuffer renderingBuffer)
		{
			renderer.Clear(Color.TransparentBlack);
			
			// Draw random polygons
			for (int i = 0; i < polygons.Length; i++)
			{
				Polygon polygon = polygons[i];

				// Make the polygon. One can call MoveTo() more than once. 
				// In this case the rasterizer behaves like Win32 API PolyPolygon().
				rasterizer.MoveToD(polygon.Points[0], polygon.Points[1]);

				for (int j = 1; j < polygon.Points.Length / 2; j++)
				{
					rasterizer.LineToD(polygon.Points[2 * j + 0], polygon.Points[2 * j + 1]);
				}

				// Render
				rasterizer.Render(renderer, polygon.Color);
			}
		}

		double Random(double min, double max)
		{
			return min + rng.NextDouble() * (max - min);
		}
	}
}
