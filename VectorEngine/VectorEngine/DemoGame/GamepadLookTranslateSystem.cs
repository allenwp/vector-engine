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
    public class GamepadLookTranslateSystem : ECSSystem
    {
        public override void Tick()
        {
            GamePadState gamePadState = EntityAdmin.Instance.SingletonGamepad.GamepadState;
            foreach ((Transform transform, GamepadLookTranslate movement) in EntityAdmin.Instance.GetTuple<Transform, GamepadLookTranslate>())
            {
                var pos = transform.Position;

                pos.X += gamePadState.ThumbSticks.Left.X * movement.TranslateSpeed * GameTime.LastFrameTime;
                pos.Z -= gamePadState.ThumbSticks.Left.Y * movement.TranslateSpeed * GameTime.LastFrameTime;

                transform.Position = pos;

                movement.Yaw += gamePadState.ThumbSticks.Right.X * movement.RotateSpeed * GameTime.LastFrameTime;
                movement.Pitch += gamePadState.ThumbSticks.Right.Y * movement.RotateSpeed * GameTime.LastFrameTime;

                transform.Rotation = Quaternion.CreateFromYawPitchRoll(movement.Yaw, movement.Pitch, 0);
            }
        }
    }
}
