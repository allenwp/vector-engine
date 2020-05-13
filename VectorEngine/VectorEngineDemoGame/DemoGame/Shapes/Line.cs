using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Output;

namespace VectorEngine.DemoGame.Shapes
{
    public class Line : Shape
    {
        public float BaseFidelity;

        public Vector3 Start;
        public Vector3 End;

        public Line()
        {
            BaseFidelity = 130;
        }

        public Line(float baseFidelity)
        {
            BaseFidelity = baseFidelity;
        }

        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int sampleLength = (int)Math.Round(BaseFidelity * fidelity);

            Sample3D[] sample3DArray = new Sample3D[sampleLength];
            for(int i = 0; i < sampleLength; i++)
            {
                var point3D = Vector3.Lerp(Start, End, (float)i / (float)(sampleLength - 1));
                sample3DArray[i].Position = point3D;
                sample3DArray[i].Brightness = 1f;
            }
            result.Add(sample3DArray);

            return result;
        }
    }
}
