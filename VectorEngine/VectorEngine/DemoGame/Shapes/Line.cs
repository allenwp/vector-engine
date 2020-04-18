using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;
using VectorEngine.Output;

namespace VectorEngine.DemoGame.Shapes
{
    public class Line : Shape
    {
        public int LineLength = 130;

        public Vector3 Start;
        public Vector3 End;

        public override List<Sample3DStream> GetSamples3D(float fidelity)
        {
            List<Sample3DStream> result = new List<Sample3DStream>(1);
            int sampleLength = (int)Math.Round(LineLength * fidelity);

            Sample3DStream sampleStream = Sample3DPool.GetStream(sampleLength);
            for(int i = 0; i < sampleLength; i++)
            {
                var point3D = Vector3.Lerp(Start, End, (float)i / (float)(sampleLength - 1));
                sampleStream[i] = new Sample3D() { Position = point3D, Brightness = 1f };
            }
            result.Add(sampleStream);

            return result;
        }
    }
}
