using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame.Shapes
{
    public class CurlySpire : Shape
    {
        public int CurlCount { get; set; } = 5;
        public float BaseFidelity { get; set; } = 300f;

        private float height = 1f; // scale with Transform if you want to change this.

        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            int sampleCount = (int)Math.Round(BaseFidelity * fidelity);
            Sample3D[] samples = new Sample3D[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float value = (float)i / (sampleCount - 1);
                float valueRadians = (float)(value * Math.PI * 2 * CurlCount);
                float widthValue = value * 2f;
                if (value > 0.5f)
                {
                    widthValue = (widthValue - 2f) * -1f;
                }

                samples[i].Position.Y = value * height;
                samples[i].Position.X = (float)Math.Sin(valueRadians) * widthValue;
                samples[i].Position.Z = (float)Math.Cos(valueRadians) * widthValue;
                samples[i].Brightness = 1f;
            }
            return new List<Sample3D[]>(1) { samples };
        }
    }
}
