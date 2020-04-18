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
        int baseSampleCount = 1000;
        public override List<Sample3DStream> GetSamples3D(float fidelity)
        {
            List<Sample3DStream> result = new List<Sample3DStream>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);

            Sample3DStream sampleStream = Sample3DPool.GetStream(sampleCount);
            for (int i = 0; i < sampleCount; i++)
            {
                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), (float)i / (float)(sampleCount - 1));
                var point3D = new Vector3((float)Math.Sin(value), (float)Math.Cos(value), 0);
                sampleStream[i] = new Sample3D() { Position = point3D, Brightness = 1f };
            }
            result.Add(sampleStream);

            return result;
        }
    }
}
