using Microsoft.Xna.Framework;
using System;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class RotateSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach ((var rotate, var transform) in EntityAdmin.Instance.GetTuple<Rotate, Transform>())
            {
                rotate.LerpAmount += rotate.Speed * GameTime.LastFrameTime;
                if (rotate.LerpAmount > 1f)
                {
                    rotate.LerpAmount -= 1f;
                }
                float rotationAmount = MathHelper.LerpPrecise(0, (float)(Math.PI * 2), rotate.LerpAmount);
                switch (rotate.Axis)
                {
                    case Rotate.AxisEnum.x:
                        transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(0, rotationAmount, 0);
                        break;
                    case Rotate.AxisEnum.y:
                        transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(rotationAmount, 0 , 0);
                        break;
                    case Rotate.AxisEnum.z:
                        transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(0, 0, rotationAmount);
                        break;
                }
            }
        }
    }
}
