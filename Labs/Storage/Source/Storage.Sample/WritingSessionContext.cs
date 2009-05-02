namespace Bnoerj.Storage.Sample
{
	using System.IO;
	using Microsoft.Xna.Framework;

	class WritingSessionContext : IStorageSessionContext
	{
		Game1 game;
		string text;

		public WritingSessionContext(Game1 game, PlayerIndex player, string text)
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
			get { return StorageSessionCanceledBehavior.Warn; }
		}

		public void SessionOpened(StorageSession session)
		{
			game.AppendStatusLine("Created.");

			var path = session.GetFullPathName("Test.txt");
			if (File.Exists(path) == true)
			{
				File.Delete(path);
			}

			using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
			{
				var writer = new StreamWriter(stream);
				writer.Write(text);
				writer.Close();
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
