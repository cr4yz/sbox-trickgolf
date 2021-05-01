using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Trickgolf.UI;

namespace Trickgolf
{
	[Library("trickgolf", Title = "Trickgolf")]
	partial class TrickgolfGame : Game
	{
		public TrickgolfGame()
		{
			if (IsServer)
            {
				new TrickgolfHud();
            }

            BetterChatbox.OnChatCommand += BetterChatbox_OnChatCommand;
		}

        private void BetterChatbox_OnChatCommand(Player player, string command)
        {
			switch(command)
            {
				case "r":
					(player as GolfPlayer).Respawn();
					break;
            }
        }

        public override Player CreatePlayer() => new GolfPlayer();

		[Event("server.tick")]
		public void OnTick()
        {
			foreach(var ball in All.OfType<PlayerBall>())
            {
				var wasMoving = ball.IsMoving;
				ball.IsMoving = !ball.Velocity.IsNearlyZero();

				if (!ball.IsMoving && wasMoving)
                {
					OnBallStoppedMoving(ball);
                }
			}
		}

		public override void DoPlayerDevCam(Player player)
		{
			if (!player.HasPermission("devcam"))
            {
				return;
            }

			if (player is GolfPlayer basePlayer)
			{
				if (basePlayer.DevCamera is DevCamera)
				{
					basePlayer.DevCamera = null;
				}
				else
				{
					basePlayer.DevCamera = new DevCamera();
				}
			}
		}
	}
}