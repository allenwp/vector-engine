using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine.Util
{
    public static class SampleUtil
    {
        public static float DistanceBetweenSamples(Sample sample1, Sample sample2)
        {
            return Vector2.Distance(new Vector2(sample1.X, sample1.Y), new Vector2(sample2.X, sample2.Y));
        }
    }
}
