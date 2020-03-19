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
            // TODO: something like this for each entity that has a Rotate and Transform
            //lerpAmount += 0.3f * GameTime.LastFrameTime;
            //if (lerpAmount > 1f)
            //{
            //    lerpAmount -= 1f;
            //}
            //foreach (var shape in shapes)
            //{
            //    Cube cube = shape as Cube;
            //    if (cube != null)
            //    {
            //        cube.transform.Rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.LerpPrecise(0, (float)(Math.PI * 2), lerpAmount), 0, 0);
            //    }
            //}
        }
    }
}
