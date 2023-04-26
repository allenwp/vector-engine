using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
	public class PlayerGamepadControlsSystem : ECSSystem
	{
		public override void Tick()
		{
			GamePadState gamePadState = EntityAdmin.Instance.GetComponents<GamepadSingleton>().First().GamepadState;
			foreach ((var transform, var playerControls) in EntityAdmin.Instance.GetTuple<Transform, PlayerGamepadControls>())
			{
				Vector2 input = new Vector2(gamePadState.ThumbSticks.Left.Y, gamePadState.ThumbSticks.Left.X * -1.0f);

				var target_pitch_yaw = playerControls.max_pitch_yaw * input;

				if (transform.LocalPosition.Y < playerControls.track_ground + 0.1f)
				{
					target_pitch_yaw.X = MathHelper.Max(0.0f, target_pitch_yaw.X);
				}

				playerControls.ship_pitch_yaw = speed_limited_tween(playerControls.ship_pitch_yaw, target_pitch_yaw, playerControls.settle_pitch_yaw_tween_steps, playerControls.max_rotate_speed);

				transform.LocalPosition.X += playerControls.ship_pitch_yaw.Y * -1.0f * playerControls.translate_speed;
				transform.LocalPosition.Y += playerControls.ship_pitch_yaw.X * playerControls.translate_speed;
				transform.LocalPosition.X = MathHelper.Clamp(transform.LocalPosition.X, playerControls.track_bounds.X / -2.0f, playerControls.track_bounds.X / 2.0f);
				transform.LocalPosition.Y = MathHelper.Clamp(transform.LocalPosition.Y, Math.Max(playerControls.track_bounds.Y / -2.0f, playerControls.track_ground), playerControls.track_bounds.Y / 2.0f);

				transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(playerControls.ship_pitch_yaw.Y, playerControls.ship_pitch_yaw.X, playerControls.ship_pitch_yaw.Y);

				//var camera_bounds: Vector2 = dolly.bounds - camera_bounds_reduction
				//var ship_position_ratio: Vector2 = Vector2(player_ship.position.x / (dolly.bounds.x / 2.0), player_ship.position.y / (dolly.bounds.y / 2.0))
				//player_camera.position.x = ship_position_ratio.x * (camera_bounds.x / 2.0)
				//player_camera.position.y = ship_position_ratio.y * (camera_bounds.y / 2.0)

			}
		}

		// A smaller num_steps will make the tween happen faster.
		// max_speed will limit the speed of the tween.
		// A smaller num_steps will make the tween happen faster.
		private Vector2 speed_limited_tween(Vector2 start, Vector2 end, float num_steps, float max_speed)
		{
			var delta = (end - start) / num_steps;
			delta = new Vector2(MathHelper.Clamp(delta.X, -1 * max_speed, max_speed),
					MathHelper.Clamp(delta.Y, -1 * max_speed, max_speed));
			return start + delta;
		}
	}
}
