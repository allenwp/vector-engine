using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.Extras
{
    public class GamepadBasicFPSMovementSystem : ECSSystem
    {
        public override void Tick()
        {
            GamePadState gamePadState = EntityAdmin.Instance.SingletonGamepad.GamepadState;
            foreach ((var transform, var movement) in EntityAdmin.Instance.GetTuple<Transform, GamepadBasicFPSMovement>())
            {
                float gameTime = movement.UseRealTime ? GameTime.LastRealFrameTime : GameTime.LastFrameTime;

                var rotateSpeed = movement.RotateSpeed * gameTime;
                movement.Yaw -= gamePadState.ThumbSticks.Right.X * rotateSpeed;
                movement.Pitch += gamePadState.ThumbSticks.Right.Y * rotateSpeed;
                movement.Roll -= gamePadState.Buttons.RightShoulder == ButtonState.Pressed ? rotateSpeed : 0;
                movement.Roll += gamePadState.Buttons.LeftShoulder == ButtonState.Pressed ? rotateSpeed : 0;
                transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(movement.Yaw, movement.Pitch, movement.Roll);

                var movementSpeed = movement.TranslateSpeed * gameTime;
                var changeX = gamePadState.ThumbSticks.Left.X * movementSpeed;
                var changeZ = -1 * gamePadState.ThumbSticks.Left.Y * movementSpeed;
                var changeY = gamePadState.Triggers.Right * movementSpeed;
                changeY -= gamePadState.Triggers.Left * movementSpeed;
                var change = new Vector3(changeX, changeY, changeZ);
                change = Vector3.Transform(change, transform.LocalRotation);

                transform.LocalPosition = transform.LocalPosition + change;
            }
        }
    }
}
