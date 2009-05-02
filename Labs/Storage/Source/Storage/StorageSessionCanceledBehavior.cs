namespace Bnoerj.Storage
{
	/// <summary>
	/// Defines the behavior when a storage selector has been cancled.
	/// </summary>
	public enum StorageSessionCanceledBehavior
	{
		/// <summary>
		/// Close the session.
		/// </summary>
		Close,

		/// <summary>
		/// Display a warning message box to the user.
		/// </summary>
		Warn,

		/// <summary>
		/// Display a message box and force the user to select a storage device.
		/// </summary>
		Force,
	}
}
