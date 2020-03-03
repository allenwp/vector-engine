using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.Output
{
    public class FrameOutput
    {
        public static readonly int SAMPLES_PER_SECOND = 192000;
        public static readonly int FRAMES_PER_SECOND = 80;
        public static readonly int BUFFER_SIZE = SAMPLES_PER_SECOND / FRAMES_PER_SECOND;

        public static Sample[] Buffer1 = new Sample[BUFFER_SIZE];
        public static Sample[] Buffer2 = new Sample[BUFFER_SIZE];

        public enum ReadStateEnum
        {
            ReadingBuffer1 = 0,
            WaitingForBuffer2,
            ReadingBuffer2,
            WaitingForBuffer1
        }
        public static volatile int ReadState = (int)ReadStateEnum.WaitingForBuffer1;

        public enum WriteStateEnum
        {
            WrittingBuffer1 = 0,
            WaitingForBuffer2,
            WrittingBuffer2,
            WaitingForBuffer1
        }
        public static volatile int WriteState = (int)WriteStateEnum.WaitingForBuffer1;

        public static void ClearBuffer(Sample[] buffer)
        {
            for(int i = 0; i < buffer.Length; i++)
            {
                buffer[i].X = -1f;
                buffer[i].Y = -1f;
                buffer[i].Brightness = 0f;
            }
        }
    }
}
