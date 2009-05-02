namespace Bnoerj.Storage
{
	using System;
	using Microsoft.Xna.Framework.Storage;

	/// <summary>
	/// Provides methods and properties for storage access.
	/// </summary>
	public class StorageSession : IDisposable
	{
		/// <summary>
		/// The storage container used by this session.
		/// </summary>
		StorageContainer container;

		/// <summary>
		/// Initializes a new instance of the StorageContainerSession class.
		/// </summary>
		/// <param name="provider">The associated storage provider.</param>
		/// <param name="container">The associated storage container.</param>
		internal StorageSession(StorageProvider provider, StorageContainer container)
		{
			this.Provider = provider;
			this.container = container;
		}

		/// <summary>
		/// Finalizes the StorageSession.
		/// </summary>
		~StorageSession()
		{
			Dispose(false);
		}

		/// <summary>
		/// Gets the StorageProvider associated with the session.
		/// </summary>
		public StorageProvider Provider
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the fill path name for the specified relative path.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <returns>The full path.</returns>
		public String GetFullPathName(string path)
		{
			return System.IO.Path.Combine(container.Path, path);
		}

		/// <summary>
		/// Immediately releases the unmanaged resources used by this object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the GameComponent and
		/// optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing == true && container != null && container.IsDisposed == false)
			{
				container.Dispose();
				container = null;
			}
		}
	}
}
