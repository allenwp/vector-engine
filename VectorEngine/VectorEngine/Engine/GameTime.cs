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

        /// <summary>
        /// In seconds, based on LastFrameSampleCount
        /// </summary>
        public static float LastFrameTime { get { return (float)LastFrameSampleCount / FrameOutput.SAMPLES_PER_SECOND; } }
    }
}
