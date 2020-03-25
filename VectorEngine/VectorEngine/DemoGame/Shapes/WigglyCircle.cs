using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class WigglyCircle : Shape
    {
        public int WiggleCount = 10;
        public int ZWiggleCount = 10;
        public float WiggleSize = 0.07f;
        public float ZWiggleSize = 0.04f;

        int baseSampleCount = 200;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            Sample3D[] sample3DArray = new Sample3D[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), (float)i / (float)(sampleCount - 1));
                var point3D = new Vector3((float)Math.Sin(value), (float)Math.Cos(value), 0);

                var wiggleValue = MathHelper.Lerp(0, (float)(Math.PI * 2), (float)i / (float)(sampleCount - 1) * WiggleCount);
                point3D += point3D * WiggleSize * (float)Math.Sin(wiggleValue);

                var zWiggleValue = MathHelper.Lerp(0, (float)(Math.PI * 2), (float)i / (float)(sampleCount - 1) * ZWiggleCount);
                point3D.Z = ZWiggleSize * (float)Math.Sin(zWiggleValue);

                sample3DArray[i].Position = point3D;
                sample3DArray[i].Brightness = 1f;
            }
            result.Add(sample3DArray);
            return result;
        }
    }
}
