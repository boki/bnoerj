namespace Bnoerj.Storage
{
	using Microsoft.Xna.Framework;

	/// <summary>
	/// Defines methods and properties to interact with StorageSessions.
	/// </summary>
	public interface IStorageSessionContext
	{
		/// <summary>
		/// The PlayerIndex to use for global message boxes.
		/// </summary>
		PlayerIndex ControllingPlayer { get; }

		/// <summary>
		/// Gets the StorageRequirements for the session.
		/// </summary>
		StorageRequirements Requirements { get; }

		/// <summary>
		/// Gets the canncelation behavior for the session.
		/// </summary>
		StorageSessionCanceledBehavior CanceledBehavior { get; }

		/// <summary>
		/// Called after the session has been opened.
		/// </summary>
		/// <param name="session"></param>
		void SessionOpened(StorageSession session);

		/// <summary>
		/// Called when the session has been canceled.
		/// </summary>
		void SessionCanceled();
	}
}
