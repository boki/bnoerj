namespace Bnoerj.Helpers
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.GamerServices;

	static class PlayerIndexExtensions
	{
		/// <summary>
		/// Retrieves a value indicating whether the player can buy the game,
		/// e.g. if the signed in player has the privileges and the game is in
		/// trial mode.
		/// </summary>
		/// <param name="player">The player index.</param>
		/// <returns>true if the player can buy the game; false otherwise.</returns>
		public static bool CanBuyGame(this PlayerIndex player)
		{
			SignedInGamer gamer = Gamer.SignedInGamers[player];
			return
				gamer != null &&
				gamer.IsSignedInToLive == true &&
				Guide.IsTrialMode == true &&
				gamer.Privileges.AllowPurchaseContent;
		}

		public static void SetPresenceMode(this PlayerIndex player, GamerPresenceMode presenceMode)
		{
			SignedInGamer gamer = Gamer.SignedInGamers[player];
			if (gamer != null)
			{
				gamer.Presence.PresenceMode = presenceMode;
			}
		}

		public static bool IsSignedIn(this PlayerIndex player)
		{
			return Gamer.SignedInGamers[player] != null;
		}
	}
}
