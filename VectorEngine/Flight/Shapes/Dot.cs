using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight.Shapes
{
    /// <summary>
    /// A fidelity-independent dot of a specified sample count.
    /// </summary>
    public class Dot : Shape
    {
        public int SampleCount { get; set; } = 2;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            var samples = new List<Sample3D[]>(1);
            var array = new Sample3D[SampleCount];
            for (int i = 0; i < SampleCount; i++)
            {
                array[i].Brightness = 1f;
            }
            samples.Add(array);
            return samples;
        }
    }
}
