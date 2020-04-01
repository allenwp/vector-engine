using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    /// <summary>
    /// A single sample, in screen space, to be passed on to the vector display.
    /// </summary>
    public struct Sample
    {
        /// <summary>
        /// 0 is the center of the screen. This should have the range of -AspectRatio to AspectRatio
        /// to be visible on the screen.
        /// A square aspect ratio will have the rage of -1 to 1 be visible.
        /// </summary>
        public float X;

        /// <summary>
        /// 0 is the center of the screen. This should be in range of -1 to 1 to be visible, regardless
        /// of aspect ratio.
        /// </summary>
        public float Y;

        /// <summary>
        /// Range of 0 to 1 where 0 is no brightness, 1 is full brightness.
        /// TODO: Figure out if it makes sense to have padding for faster changing.
        /// For example, maybe 0.1 is no brightness, but 0 will drive the signal faser to no brightness
        /// and vice-versa.
        /// </summary>
        public float Brightness;

        public static Sample Blank
        {
            get
            {
                Sample blank = new Sample(-1f, -1f, 0);
                blank.X *= FrameOutput.AspectRatio;
                return blank;
            }
        }

        public Sample(float x = 0f, float y = 0f, float brightness = 1f)
        {
            X = x;
            Y = y;
            Brightness = brightness;
        }

        /// <summary>
        /// Clamps the sample to (-1, -1) and (1, 1)
        /// </summary>
        public void Clamp()
        {
            if (X < -1f)
            {
                X = -1f;
            }
            else if (X > 1f)
            {
                X = 1f;
            }

            if (Y < -1f)
            {
                Y = -1f;
            }
            else if (Y > 1f)
            {
                Y = 1f;
            }
        }
    }
}
