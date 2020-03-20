using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class RotateSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach ((Rotate rotate, Transform transform) in EntityAdmin.Instance.GetTuple<Rotate, Transform>())
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
                        transform.Rotation = Quaternion.CreateFromYawPitchRoll(rotationAmount, 0, 0);
                        break;
                    case Rotate.AxisEnum.y:
                        transform.Rotation = Quaternion.CreateFromYawPitchRoll(0, rotationAmount, 0);
                        break;
                    case Rotate.AxisEnum.z:
                        transform.Rotation = Quaternion.CreateFromYawPitchRoll(0, 0, rotationAmount);
                        break;
                }
            }
        }
    }
}
