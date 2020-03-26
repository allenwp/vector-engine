using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class CurlyCircle : Shape
    {
        public int CurlCount = 20;
        public float CurlSize = 0.2f;

        int baseSampleCount = 1000;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            Sample3D[] sample3DArray = new Sample3D[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), (float)i / (float)(sampleCount - 1));
                var point3D = new Vector3((float)Math.Sin(value), (float)Math.Cos(value), 0);

                var secondValue = MathHelper.Lerp(0, (float)(Math.PI * 2), (float)i / (float)(sampleCount - 1) * CurlCount);
                var secondCirclePoint = CurlSize * new Vector3((float)Math.Sin(secondValue), (float)Math.Cos(secondValue), 0);
                point3D += secondCirclePoint;

                sample3DArray[i].Position = point3D;
                sample3DArray[i].Brightness = 1f;
            }
            result.Add(sample3DArray);
            return result;
        }
    }
}
