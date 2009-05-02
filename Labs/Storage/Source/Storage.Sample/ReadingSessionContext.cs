namespace Bnoerj.Storage.Sample
{
	using System.IO;
	using System.Text;
	using Microsoft.Xna.Framework;

	/// <summary>
	/// Defines methods and properties for the reading sessions.
	/// </summary>
	class ReadingSessionContext : IStorageSessionContext
	{
		Game1 game;
		StringBuilder text;

		public ReadingSessionContext(Game1 game, PlayerIndex player, StringBuilder text)
		{
			this.game = game;
			this.text = text;
			this.ControllingPlayer = player;
			this.Requirements = new StorageRequirements(player);
		}

		public PlayerIndex ControllingPlayer
		{
			get;
			private set;
		}

		public StorageRequirements Requirements
		{
			get;
			private set;
		}

		public StorageSessionCanceledBehavior CanceledBehavior
		{
			get { return StorageSessionCanceledBehavior.Close; }
		}

		public void SessionOpened(StorageSession session)
		{
			game.AppendStatusLine("Created.");

			var path = session.GetFullPathName("Test.txt");
			if (File.Exists(path) == true)
			{
				text.Length = 0;
				using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					var reader = new StreamReader(stream);
					text.Append(reader.ReadToEnd());
				}
			}

			game.AppendStatusLine("Done.");
			game.ContainerSessionFinished();
		}

		public void SessionCanceled()
		{
			game.AppendStatusLine("Cancelled.");
			game.ContainerSessionFinished();
		}
	}
}
