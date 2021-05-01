using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace Trickgolf
{
	[Library("trickgolf", Title = "Trickgolf")]
	partial class TrickgolfGame : Game
	{
		public TrickgolfGame()
		{
			// easy way for now.. todo look into actual clientside huds?
			if (IsServer)
            {
				new TrickgolfHud();
            }

			_ = StartTickTimer();
		}

		public override Player CreatePlayer() => new GolfPlayer();

		public async Task StartTickTimer()
		{
			while (true)
			{
				await Task.NextPhysicsFrame();
				OnTick();
			}
		}

		public void OnTick()
        {
			if (Host.IsClient)
            {
				return;
            }

			foreach(var ball in All.OfType<PlayerBall>())
            {
				var wasMoving = ball.IsMoving;
				ball.IsMoving = !ball.Velocity.IsNearlyZero();

				if (ball.IsMoving == false && wasMoving == true)
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