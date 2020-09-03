using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Calibration.Shapes
{
    public class BrightnessTestCircle : Shape
    {
        public int SamplesPerQuarterCircle = 4;
        public int BlankingCircles = 3;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int numQuarterCircles = (4 * BlankingCircles) + 4 + (4 * BlankingCircles);
            List<double> doubles = new List<double>();

            Sample3D[] array = new Sample3D[numQuarterCircles * SamplesPerQuarterCircle];
            for (int i = 0; i < array.Length; i++)
            {
                double radians = i * (MathHelper.PiOver2 / SamplesPerQuarterCircle);
                doubles.Add(radians);
                float expansion = 1f;// + i * 0.001f;
                array[i].Position = new Vector3((float)Math.Sin(radians) * expansion, (float)Math.Cos(radians) * expansion, 0);
                array[i].Brightness = 1f;
            }

            result.Add(array);
            return result;
        }
    }
}
