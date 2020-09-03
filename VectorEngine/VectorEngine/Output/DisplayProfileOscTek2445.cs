using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Util;

namespace VectorEngine.Output
{
    /// <summary>
    /// Display Profile for the Tektronix 2445 Oscilloscope
    /// </summary>
    public class DisplayProfileOscTek2445 : DisplayProfile
    {
        public override float AspectRatio { get; set; } = 1.26f;

        public override float FullBrightnessOutput { get; set; } = -1f;

        public override float ZeroBrightnessOutput { get; set; } = 1f;

        // TODO: maybe change the blanking length thing into something that actually returns
        // the blanking array between two samples? This way the whole blanking algorithm
        // could be display specific.
        /// <summary>
        /// Number of samples for each blank at a distance of 1 unit between samples
        /// </summary>
        private static float blankingLength = 14f;
        public override int BlankingLength(Sample sample1, Sample sample2)
        {
            // Clamp these because that's what's going to happen at output time anyway
            sample1.Clamp();
            sample2.Clamp();
            var distance = SampleUtil.DistanceBetweenSamples(sample1, sample2);
            if (distance < 0.01f)
            {
                // they're so close together, no blanking is needed.
                return 0;
            }
            else
            {
                return (int)Math.Ceiling(blankingLength * distance);
            }
        }

        public override float FidelityScale { get; set; } = 1f;
    }
}
