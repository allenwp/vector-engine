using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class GamepadBasicFPSMovementSystem : ECSSystem
    {
        public override void Tick()
        {
            GamePadState gamePadState = EntityAdmin.Instance.SingletonGamepad.GamepadState;
            foreach ((Transform transform, GamepadBasicFPSMovement movement) in EntityAdmin.Instance.GetTuple<Transform, GamepadBasicFPSMovement>())
            {
                movement.Yaw -= gamePadState.ThumbSticks.Right.X * movement.RotateSpeed * GameTime.LastFrameTime;
                movement.Pitch += gamePadState.ThumbSticks.Right.Y * movement.RotateSpeed * GameTime.LastFrameTime;
                movement.Roll -= gamePadState.Triggers.Right * movement.RotateSpeed * GameTime.LastFrameTime;
                movement.Roll += gamePadState.Triggers.Left * movement.RotateSpeed * GameTime.LastFrameTime;
                transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(movement.Yaw, movement.Pitch, movement.Roll);

                var changeX = gamePadState.ThumbSticks.Left.X * movement.TranslateSpeed * GameTime.LastFrameTime;
                var changeZ = -1 * gamePadState.ThumbSticks.Left.Y * movement.TranslateSpeed * GameTime.LastFrameTime;
                var change = new Vector3(changeX, 0, changeZ);
                change = Vector3.Transform(change, transform.LocalRotation);

                transform.LocalPosition = transform.LocalPosition + change;
            }
        }
    }
}
