namespace Bnoerj.Storage
{
	using System.Collections.Generic;
	using Microsoft.Xna.Framework;

	/// <summary>
	/// Defines methods and properties to access StorageProviders and
	/// StorageSessions.
	/// </summary>
	public class StorageService : GameComponent
	{
		/// <summary>
		/// The title name.
		/// </summary>
		string titleName;

		/// <summary>
		/// The list of requested StorageProviders.
		/// </summary>
		List<StorageProvider> providers;

		/// <summary>
		/// Initializes a new instance of the StorageService class.
		/// </summary>
		/// <param name="game">The game instance.</param>
		/// <param name="titleName">The title name.</param>
		public StorageService(Game game, string titleName)
			: base(game)
		{
			this.titleName = titleName;
			this.providers = new List<StorageProvider>(5);
		}

		/// <summary>
		/// Retrieves a provider satisfying the specified requirements.
		/// </summary>
		/// <param name="requirements">The requirements.</param>
		/// <returns>The StorageProvider instance.</returns>
		public StorageProvider GetProvider(StorageRequirements requirements)
		{
			var provider = providers.Find(s => s.MatchesRequirements(requirements));
			if (provider == null)
			{
				provider = new StorageProvider(titleName, requirements);
				providers.Add(provider);
			}

			return provider;
		}

		/// <summary>
		/// Opens a session.
		/// </summary>
		/// <param name="context">An instance of IStorageSessionContext defining the requirements and callbacks.</param>
		public void OpenSession(IStorageSessionContext context)
		{
			var provider = GetProvider(context.Requirements);
			provider.OpenSession(context);
		}

		/// <summary>
		/// Updates the StorageService.
		/// </summary>
		/// <param name="gameTime">Time elapsed since the last call to Update.</param>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			for (int i = 0; i < providers.Count; i++)
			{
				providers[i].Update();
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the GameComponent and
		/// optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
