using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.Output
{
    public abstract class DisplayProfile
    {
        public abstract float AspectRatio { get; }
        public abstract int BlankingLength(Sample sample1, Sample sample2);
    }
}
