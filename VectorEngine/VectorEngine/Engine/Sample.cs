using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    /// <summary>
    /// A single sample, in screen space, to be passed on to the vector display.
    /// </summary>
    public struct Sample
    {
        /// <summary>
        /// 0 is the center of the screen. Unbounded to support non-square screen aspect ratios.
        /// If a screen is portrait aspect ratio, this should be in range of -1 to 1 to be visible.
        /// </summary>
        public float X;

        /// <summary>
        /// 0 is the center of the screen. Unbounded to support non-square screen aspect ratios.
        /// If a screen is landscape aspect ratio, this should be in range of -1 to 1 to be visible.
        /// </summary>
        public float Y;

        /// <summary>
        /// Range of 0 to 1 where 0 is no brightness, 1 is full brightness.
        /// TODO: Figure out if it makes sense to have padding for faster changing.
        /// For example, maybe 0.1 is no brightness, but 0 will drive the signal faser to no brightness
        /// and vice-versa.
        /// </summary>
        public float Brightness;
    }
}
