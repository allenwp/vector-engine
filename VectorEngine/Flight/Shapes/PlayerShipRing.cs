using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight.Shapes
{
    public class PlayerShipRing : Shape
    {
        public float BaseFidelity { get; set; } = 100f;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            int sampleCount = (int)Math.Round(BaseFidelity * fidelity);
            var samplesArray = new Sample3D[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var value = ((float)i / (sampleCount - 1)) * MathHelper.TwoPi;
                samplesArray[i].Brightness = 1f;
                samplesArray[i].Position.X = (float)Math.Sin(value);
                samplesArray[i].Position.Y = (float)Math.Cos(value);
            }
            var result = new List<Sample3D[]>(1);
            result.Add(samplesArray);
            return result;
        }
    }
}
