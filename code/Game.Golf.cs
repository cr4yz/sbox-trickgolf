﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace Minigolf
{
	partial class GolfGame
	{
		[ServerVar("minigolf_power_multiplier")]
		public static float PowerMultiplier { get; set; } = 25.0f;

		// todo: move this stuff out!!
		[Net] public int CurrentHole { get; set; } = 1;
		public int HolePar { get; set; } = 2;

		static readonly SoundEvent PuttSound = new SoundEvent("sounds/ballinhole.vsnd");

		static readonly SoundEvent[][] SwingSounds = new SoundEvent[][] {
			new SoundEvent[] {
				new("sounds/golfswing_supersoft_01.vsnd"),
				new("sounds/golfswing_supersoft_02.vsnd"),
				new("sounds/golfswing_supersoft_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/golfswing_soft_01.vsnd"),
				new("sounds/golfswing_soft_02.vsnd"),
				new("sounds/golfswing_soft_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/golfswing_medium_01.vsnd"),
				new("sounds/golfswing_medium_02.vsnd"),
				new("sounds/golfswing_medium_03.vsnd"),
			},
			new SoundEvent[] {
				new("sounds/golfswing_hard_01.vsnd"),
				new("sounds/golfswing_hard_02.vsnd"),
				new("sounds/golfswing_hard_03.vsnd"),
			},
		};

		/// <summary>
		/// Reset's the ball to the spawn point of the current hole.
		/// </summary>
		/// <param name="ball"></param>
		public void ResetBall(PlayerBall ball)
        {
			var spawn = Entity.All.OfType<BallSpawn>().Where(x => x.Hole == CurrentHole).FirstOrDefault();
			if (spawn == null) return;

			// todo: trace up

			// Reset all velocity
			ball.PhysicsBody.Velocity = Vector3.Zero;
			ball.PhysicsBody.AngularVelocity = Vector3.Zero;

			ball.WorldPos = spawn.WorldPos;

			ball.IsMoving = false;
			ball.InHole = false;
		}

		public void OnBallStoppedMoving(PlayerBall ball)
		{
			if (!ball.InHole && !HoleInfo.InBounds(CurrentHole, ball))
				BallOutOfBounds(ball);
		}

		public void BallOutOfBounds(PlayerBall ball)
        {
			ResetBall(ball);

			// Tell the ball owner his balls are out of bounds
			ClientBallOutOfBounds(ball.Owner, ball);
		}

		[ClientRpc]
		public void ClientBallOutOfBounds(PlayerBall ball)
		{
			_ = OutOfBounds.Current.Show();
		}

		public void OnBallInHole(PlayerBall ball, int hole)
        {
			var player = ball.Owner as GolfPlayer;

			ball.InHole = true;
			ball.PlaySound(PuttSound.Name);

			// Announce to all players
			Sandbox.UI.ChatBox.AddInformation(Player.All, $"{player.Name} putted on hole {hole}!", $"avatar:{player.SteamId}");
			PlayerBallInHole(ball, player.Strokes);

			// await Task.DelaySeconds(5);

			// Advance hole like this for now
			CurrentHole = CurrentHole == 1 ? 2 : 1;

			// Reset for now
			player.Strokes = 0;
			ResetBall(ball);
		}

		[ClientRpc]
		protected void PlayerBallInHole(PlayerBall ball, int strokes)
        {
			_ = EndScore.Current.ShowScore(CurrentHole, HolePar, strokes);
		}

		[ServerCmd("minigolf_stroke")]
		public static void PlayerBallStroke(float yaw, int power)
		{
			var owner = ConsoleSystem.Caller;

			if (owner == null && owner is GolfPlayer)
				return;

			var player = owner as GolfPlayer;

			// Don't let a player hit an already moving ball or one in the hole
			if (player.Ball.IsMoving || player.Ball.InHole)
				return;

			// Clamp the power, should be 0-100
			power = Math.Clamp(power, 0, 100);

			// remove when SoundEvents aren't fucked
			string modifier;
			if (power < 25)
				modifier = "supersoft";
			else if (power < 50)
				modifier = "soft";
			else if (power < 75)
				modifier = "medium";
			else
				modifier = "hard";

			// Smack that ball
			player.Ball.PhysicsBody.Velocity += Angles.AngleVector(new Angles(0, yaw, 0)) * (float)power * PowerMultiplier;
			player.Ball.PhysicsBody.AngularVelocity = Vector3.Zero;

			// Play the sound from where the ball was, the sound shouldn't follow the ball
			Sound.FromWorld($"golfswing_{modifier}_0{Rand.Int(1, 3)}", player.Ball.WorldPos);

			// var sound = SwingSounds[(int)MathF.Ceiling(power / 25)][Rand.Int(0, 2)];
			// Sound.FromWorld(sound.Name, player.Ball.WorldPos);

			player.Strokes++;
		}
	}
}