namespace Bnoerj.Storage.Sample
{
	using System.Text;
	using Bnoerj.Helpers;
	using Bnoerj.Storage;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.GamerServices;
	using Microsoft.Xna.Framework.Graphics;
	using Microsoft.Xna.Framework.Input;

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		const string DefaultText = "Press X to save, Y to load, A & B to change.";

		class ActiveGamePadState
		{
			public PlayerIndex Player;
			public GamePadState Current;
			public GamePadState Previous;

			public bool IsButtonDown(Buttons buttons)
			{
				return
					Current.IsButtonDown(buttons) == true &&
					Previous.IsButtonDown(buttons) == false;
			}
		}

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont font;

		StorageService storageService;
		StorageProvider globalStorageProvider;

		ActiveGamePadState activeGamePadState;
		GamePadState[] previousGamePadStates;

		IStorageSessionContext storageSessionContext;

		Vector2 position;
		StringBuilder text;
		StringBuilder statusText;

		/// <summary>
		/// Initializes a new instance of the Game1 class.
		/// </summary>
		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
#if WINDOWS
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 576;
#else
			var curDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			graphics.PreferredBackBufferWidth = curDisplayMode.Width;
			graphics.PreferredBackBufferHeight = curDisplayMode.Height;
#endif
			var gamerService = new GamerServicesComponent(this);
			Components.Add(gamerService);

			storageService = new StorageService(this, "Bnoerj Storage Sample");
			Components.Add(storageService);

			activeGamePadState = new ActiveGamePadState();
			previousGamePadStates = new GamePadState[]
			{
				new GamePadState(),
				new GamePadState(),
				new GamePadState(),
				new GamePadState(),
			};

			text = new StringBuilder();
			statusText = new StringBuilder();
		}

		public void AppendStatusLine(string text)
		{
			statusText.AppendLine(text);
		}

		public void ContainerSessionFinished()
		{
			storageSessionContext = null;
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

			var rc = GraphicsDevice.Viewport.TitleSafeArea;
			position = new Vector2()
			{
				X = rc.X,
				Y = rc.Y
			};

			globalStorageProvider = storageService.GetProvider(new StorageRequirements());
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
			if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true ||
				GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				Exit();
			}

			if (Guide.IsVisible == false &&
				storageSessionContext == null && UpdateInput() == true)
			{
				HandleInput();
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

			spriteBatch.Begin();

			var pos = position;
			spriteBatch.DrawString(font, DefaultText, pos, Color.Wheat);

			if (text.Length > 0)
			{
				pos.Y += font.LineSpacing;
				spriteBatch.DrawString(font, text, pos, Color.White);
			}

			if (statusText.Length > 0)
			{
				pos.Y = GraphicsDevice.Viewport.TitleSafeArea.Bottom -
					font.MeasureString(statusText).Y;
				spriteBatch.DrawString(font, statusText, pos, Color.Yellow);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		void HandleInput()
		{
			if (activeGamePadState.IsButtonDown(Buttons.X) == true)
			{
				if (activeGamePadState.Player.IsSignedIn() == false)
				{
					Guide.ShowSignIn(1, false);
				}

				statusText.Length = 0;
				statusText.AppendFormat("Creating writing session for {0}...",
					activeGamePadState.Player);
				statusText.AppendLine("");

				storageSessionContext = new WritingSessionContext(this, activeGamePadState.Player, text.ToString());
				storageService.OpenSession(storageSessionContext);
			}
			else if (activeGamePadState.IsButtonDown(Buttons.Y) == true)
			{
				if (activeGamePadState.Player.IsSignedIn() == false)
				{
					Guide.ShowSignIn(1, false);
				}

				statusText.Length = 0;
				statusText.AppendFormat("Creating reading session for {0}...",
					activeGamePadState.Player);
				statusText.AppendLine("");

				storageSessionContext = new ReadingSessionContext(this, activeGamePadState.Player, text);
				storageService.OpenSession(storageSessionContext);
			}
			else if (activeGamePadState.IsButtonDown(Buttons.A) == true)
			{
				if (statusText.Length == 0 || statusText.Length > 5)
				{
					statusText.Length = 0;
					statusText.Append("Dirty");
				}

				text.Append("A");
				if (text.Length % 20 == 0)
				{
					text.Append("\n");
				}
			}
			else if (activeGamePadState.IsButtonDown(Buttons.B) == true)
			{
				if (statusText.Length == 0 || statusText.Length > 5)
				{
					statusText.Length = 0;
					statusText.Append("Dirty");
				}

				text.Append("B");
				if (text.Length % 20 == 0)
				{
					text.Append("\n");
				}
			}
		}

		StorageRequirements CreateRequirements(PlayerIndex player)
		{
			return new StorageRequirements(player);
		}

		bool UpdateInput()
		{
			bool result = false;
			for (int i = 0; i < 4; i++)
			{
				var state = GamePad.GetState((PlayerIndex)i);
				var prev = previousGamePadStates[i];
				if (state.Buttons.A != prev.Buttons.A ||
					state.Buttons.B != prev.Buttons.B ||
					state.Buttons.Back != prev.Buttons.Back ||
					state.Buttons.BigButton != prev.Buttons.BigButton ||
					state.Buttons.Start != prev.Buttons.Start ||
					state.Buttons.X != prev.Buttons.X ||
					state.Buttons.Y != prev.Buttons.Y)
				{
					result = true;
					activeGamePadState.Player = (PlayerIndex)i;
					activeGamePadState.Current = state;
					activeGamePadState.Previous = prev;
				}

				previousGamePadStates[i] = state;
			}

			return result;
		}
	}
}
