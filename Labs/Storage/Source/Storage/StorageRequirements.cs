namespace Bnoerj.Storage
{
	using Microsoft.Xna.Framework;

	/// <summary>
	/// Defines properties for storage device selection.
	/// </summary>
	public struct StorageRequirements
	{
		/// <summary>
		/// Index of the player being shown the user interface display.
		/// </summary>
		public readonly PlayerIndex? Player;

		/// <summary>
		/// Size, in bytes, of the data to write to the storage device.
		/// </summary>
		public readonly int SizeInBytes;

		/// <summary>
		/// Number of directories to write to the storage device.
		/// </summary>
		public readonly int DirectoryCount;

		/// <summary>
		/// Initializes a new instance of the StorageRequirements class.
		/// </summary>
		/// <param name="playerIndex">Index of the player being shown the user interface display.</param>
		/// <param name="sizeInBytes">Size, in bytes, of the data to write to the storage device.</param>
		/// <param name="directoryCount">Number of directories to write to the storage device.</param>
		StorageRequirements(PlayerIndex? playerIndex, int sizeInBytes, int directoryCount)
		{
			Player = playerIndex;
			SizeInBytes = sizeInBytes;
			DirectoryCount = directoryCount;
		}

		/// <summary>
		/// Initializes a new instance of the StorageRequirements class.
		/// </summary>
		/// <param name="playerIndex">Index of the player being shown the user interface display.</param>
		public StorageRequirements(PlayerIndex playerIndex)
			: this(playerIndex, 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the StorageRequirements class.
		/// </summary>
		/// <param name="sizeInBytes">Size, in bytes, of the data to write to the storage device.</param>
		/// <param name="directoryCount">Number of directories to write to the storage device.</param>
		public StorageRequirements(int sizeInBytes, int directoryCount)
			: this(null, sizeInBytes, directoryCount)
		{
		}

		/// <summary>
		/// Initializes a new instance of the StorageRequirements class.
		/// </summary>
		/// <param name="playerIndex">Index of the player being shown the user interface display.</param>
		/// <param name="sizeInBytes">Size, in bytes, of the data to write to the storage device.</param>
		/// <param name="directoryCount">Number of directories to write to the storage device.</param>
		public StorageRequirements(PlayerIndex playerIndex, int sizeInBytes, int directoryCount)
			: this((PlayerIndex?)playerIndex, sizeInBytes, directoryCount)
		{
		}

		/// <summary>
		/// Compares two StorageRequirement values for equality.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>true if left and right are equal; otherwise, false.</returns>
		public static bool operator ==(StorageRequirements left, StorageRequirements right)
		{
			return 
				left.Player == right.Player &&
				left.SizeInBytes == right.SizeInBytes &&
				left.DirectoryCount == right.DirectoryCount;
		}

		/// <summary>
		/// Compares two StorageRequirement values for equality.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>true if left and right are not equal; otherwise, false.</returns>
		public static bool operator !=(StorageRequirements left, StorageRequirements right)
		{
			return (left == right) == false;
		}

		/// <summary>
		/// Returns a value that indicates whether this instance is equal to a
		/// specified object.
		/// </summary>
		/// <param name="obj">An Object to compare with this instance or null reference.</param>
		/// <returns>true if obj equals the type and value of this instance; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <remarks>
		/// This method assumes that SizeInBytes is less than 2^23, the
		/// DirectoryCount is less than 123 and the Player is less than 4.
		/// </remarks>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return (SizeInBytes << 8) | (DirectoryCount + 4) |
				(Player.HasValue ? (int)Player.Value : 0);
		}
	}
}
