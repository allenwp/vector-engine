using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class Circle : Shape
    {
        int baseSampleCount = 200;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            result.Add(GetCircle(sampleCount));
            return result;
        }

        public static Sample3D[] GetCircle(int sampleCount)
        {
            Sample3D[] sample3DArray = new Sample3D[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), (float)i / (float)(sampleCount ));
                var point3D = new Vector3((float)Math.Sin(value), (float)Math.Cos(value), 0);
                sample3DArray[i].Position = point3D;
                sample3DArray[i].Brightness = 1f;
            }
            return sample3DArray;
        }
    }
}
