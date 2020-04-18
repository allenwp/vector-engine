using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    /// <summary>
    /// Cleared every frame!
    /// </summary>
    public static class Sample3DPool
    {
        public static Sample3D[] Pool = new Sample3D[(int)(FrameOutput.SAMPLES_PER_SECOND / FrameOutput.TARGET_FRAMES_PER_SECOND) * 10]; // Start with 10 times the number of samples needed for a target frame. This gives some room to throw away a bunch of samples

        private static int startIndex = 0;

        public static Sample3DStream GetStream(int length)
        {
            if (startIndex + length > Pool.Length)
            {
                Array.Resize(ref Pool, Pool.Length + length);
            }

            Sample3DStream result = new Sample3DStream();
            result.Length = length;
            result.StartIndex = startIndex;

            startIndex += length;

            return result;
        }

        public static void ClearPool()
        {
            startIndex = 0;
        }
    }
}
