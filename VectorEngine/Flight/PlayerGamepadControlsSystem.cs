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
            GamePadState gamePadState = EntityAdmin.Instance.SingletonGamepad.GamepadState;
            foreach ((var transform, var playerControls) in EntityAdmin.Instance.GetTuple<Transform, PlayerGamepadControls>())
            {
                transform.LocalPosition.X += playerControls.Speed * GameTime.LastFrameTime * gamePadState.ThumbSticks.Left.X;
                transform.LocalPosition.Y += playerControls.Speed * GameTime.LastFrameTime * gamePadState.ThumbSticks.Left.Y;

                transform.LocalPosition.X = MathHelper.Clamp(transform.LocalPosition.X, playerControls.XMin, playerControls.XMax);
                transform.LocalPosition.Y = MathHelper.Clamp(transform.LocalPosition.Y, playerControls.YMin, playerControls.YMax);

                var roll = (gamePadState.Triggers.Left - gamePadState.Triggers.Right) * playerControls.MaxRoll;
                transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(0, 0, roll);
            }
        }
    }
}
