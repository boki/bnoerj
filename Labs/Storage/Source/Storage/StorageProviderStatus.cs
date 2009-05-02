namespace Bnoerj.Storage
{
	/// <summary>
	/// Defines the various statuses of a StorageProvider.
	/// </summary>
	public enum StorageProviderStatus
	{
		/// <summary>
		/// The storage provider has been created.
		/// </summary>
		Created,

		/// <summary>
		/// The storage provider is connected to a storage device.
		/// </summary>
		Connected,

		/// <summary>
		/// The storage providers device has been disconnected.
		/// </summary>
		Disconnected,

		/// <summary>
		/// The storage provider will show the storage device selctor as soon
		/// as possible.
		/// </summary>
		PendingSelector,

		/// <summary>
		/// The storage provider shows the storage device selector.
		/// </summary>
		ShowSelector,

		/// <summary>
		/// The storage providers storage device selector has been canceled.
		/// </summary>
		Canceled,
	}
}
