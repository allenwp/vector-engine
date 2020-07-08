using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine
{
    public class GameTime
    {
        public static int LastFrameSampleCount = 0;

        public static float TimeScale = 1f;

        /// <summary>
        /// In seconds, based on LastFrameSampleCount, but scaled by TimeScale
        /// </summary>
        public static float LastFrameTime { get { return LastRealFrameTime * TimeScale; } }

        /// <summary>
        /// In seconds, based on LastFrameSampleCount
        /// </summary>
        public static float LastRealFrameTime { get { return (float)LastFrameSampleCount / FrameOutput.SAMPLES_PER_SECOND; } }
    }
}
