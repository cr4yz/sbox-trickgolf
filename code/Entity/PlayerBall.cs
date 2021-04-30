using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minigolf
{
	/// <summary>
	/// Player golf ball
	/// </summary>
	[Library("minigolf_ball")]
	public partial class PlayerBall : ModelEntity
	{
		[Net] public bool IsMoving { get; set; }
		public bool InHole { get; set; }

		static readonly SoundEvent BounceSound = new("sounds/minigolf.ball_bounce1.vsnd");
		[Net] public Particles Trail { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/golf_ball.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;

			// Disable shadows, they seem weirdly busted.
			EnableShadowCasting = true;

			Transmit = TransmitType.Always;

			// Create a networked trail for all players to see
			Trail = Particles.Create("particles/ball_trail.vpcf");
			Trail.SetEntity(0, this);

			// fiddlsticks
			// PhysicsBody.Mass = 30.0f;
			// PhysicsBody.AngularDamping = 0.8f;
			// PhysicsBody.LinearDamping = 0.8f;
		}

		/// <summary>
		/// Do our own physics collisions, we create a fun bouncing effect this way and handle collision sounds.
		/// </summary>
		/// <param name="eventData"></param>
		protected override void OnPhysicsCollision(CollisionEventData eventData)
		{
			// Walls are non world cause it's fucked
			if (eventData.Entity.IsWorld)
				return;

			// Don't do ridiculous bounces upwards, just bounce off walls mainly
			if (Vector3.Up.Dot(eventData.Normal) >= -0.35)
            {
				var reflect = Vector3.Reflect(eventData.PreVelocity.Normal, eventData.Normal.Normal).Normal;
				var newSpeed = Math.Max(eventData.PreVelocity.Length, eventData.Speed);

				DebugOverlay.Line(eventData.Pos, eventData.Pos - (eventData.PreVelocity.Normal * 64.0f), 5);
				DebugOverlay.Line(eventData.Pos, eventData.Pos + (reflect * 64.0f), 5);

				PhysicsBody.Velocity = reflect * newSpeed * 0.8f;
				PhysicsBody.AngularVelocity = Vector3.Zero;

				var particle = Particles.Create("particles/ball_hit.vpcf", eventData.Pos);
				particle.SetPos(0, eventData.Pos);
				particle.SetForward(0, reflect);
				particle.Destroy(false);

				// Collision sound happens at this point, not entity
				var sound = Sound.FromWorld(BounceSound.Name, eventData.Pos);
				sound.SetVolume(1.0f); // todo: scale this based on speed (it can go above 1.0)
			}
		}
	}
}
