﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.Output
{
    public abstract class DisplayProfile
    {
        public abstract float AspectRatio { get; set; }
        
        /// <summary>
        /// Sample.Brightness of 1.0 will be adjusted to this output value.
        /// Usually something like -1.0 for an oscilloscope.
        /// </summary>
        public abstract float FullBrightnessOutput { get; set; }

        /// <summary>
        /// Sample.Brightness of 0.0 will be adjusted to this output value.
        /// Usually something like 1.0 for an oscilloscope.
        /// </summary>
        public abstract float ZeroBrightnessOutput { get; set; }

        public abstract int BlankingLength(Sample sample1, Sample sample2);

        public abstract float FidelityScale { get; set; }
    }
}
