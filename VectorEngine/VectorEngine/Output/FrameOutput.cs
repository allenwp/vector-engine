﻿using System;
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
        public static readonly float TARGET_FRAMES_PER_SECOND = 80f;
        public static readonly int TARGET_BUFFER_SIZE = (int)Math.Round(SAMPLES_PER_SECOND / TARGET_FRAMES_PER_SECOND);
        /// <summary>
        /// Number of samples for each blank
        /// </summary>
        public static readonly int BLANKING_LENGTH = 2;

        public static Sample[] Buffer1;
        public static Sample[] Buffer2;

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
                buffer[i] = Sample.Blank;
            }
        }
    }
}
