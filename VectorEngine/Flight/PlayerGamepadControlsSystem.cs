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
				if (playerControls.camera_transform == null)
                {
					playerControls.camera_transform = EntityAdmin.Instance.GetTuple<Transform, Camera>().First().Item1;
                }

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


				var camera_pitch_yaw = playerControls.ship_pitch_yaw * playerControls.camera_rotation_scale;
				playerControls.camera_transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(camera_pitch_yaw.Y, camera_pitch_yaw.X, camera_pitch_yaw.Y);

				Vector2 camera_bounds = playerControls.track_bounds - playerControls.camera_bounds_reduction;
				Vector2 ship_position_ratio = new Vector2(transform.LocalPosition.X / (playerControls.track_bounds.X / 2.0f), transform.LocalPosition.Y / (playerControls.track_bounds.Y / 2.0f));
				playerControls.camera_transform.LocalPosition.X = ship_position_ratio.X * (camera_bounds.X / 2.0f);
				playerControls.camera_transform.LocalPosition.Y = ship_position_ratio.Y * (camera_bounds.Y / 2.0f);
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
