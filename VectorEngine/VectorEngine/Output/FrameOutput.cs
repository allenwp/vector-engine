using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Util;

namespace VectorEngine.Output
{
    public class FrameOutput
    {
        public static readonly int SAMPLES_PER_SECOND = 192000;
        public static readonly float TARGET_FRAMES_PER_SECOND = 80f;
        public static readonly int TARGET_BUFFER_SIZE = (int)Math.Round(SAMPLES_PER_SECOND / TARGET_FRAMES_PER_SECOND);

        public static DisplayProfile DisplayProfile = new DisplayProfileOscL212();

        /// <summary>
        /// My PreSonus DAC has a delay on the channel I use for blanking -_-
        /// </summary>
        public static readonly int BLANKING_CHANNEL_DELAY = 17;

        #region Double Buffers
        public static Sample[] Buffer1;
        public static Sample[] Buffer2;

        public enum ReadStateEnum
        {
            WaitingToReadBuffer1 = 0,
            ReadingBuffer1,
            WaitingToReadBuffer2,
            ReadingBuffer2
        }
        public static volatile int ReadState = (int)ReadStateEnum.WaitingToReadBuffer1;

        public enum WriteStateEnum
        {
            WaitingToWriteBuffer1 = 0,
            WrittingBuffer1,
            WaitingToWriteBuffer2,
            WrittingBuffer2
        }
        public static volatile int WriteState = (int)WriteStateEnum.WaitingToWriteBuffer1;
        #endregion

        public static volatile int StarvedSamples = 0;

        public static ulong FrameCount = 0;

        public static void ClearBuffer(Sample[] buffer, int startIndex = 0)
        {
            for(int i = startIndex; i < buffer.Length; i++)
            {
                buffer[i] = Sample.Blank;
            }
        }

        public static Sample[] GetCalibrationFrame()
        {
            int brightestSampleCount = 300;

            Sample[] buffer = new Sample[brightestSampleCount * 8];
            ClearBuffer(buffer);

            int bufferIndex = 0;

            float aspectRatio = DisplayProfile.AspectRatio;

            bufferIndex = CalibrationDrawPoint(-aspectRatio, -1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(-aspectRatio, 0, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(-aspectRatio, 1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(0, 1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(aspectRatio, 1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(aspectRatio, 0, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(aspectRatio, -1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(0, -1, buffer, brightestSampleCount, bufferIndex);

            return buffer;
        }

        /// <returns>Next safe index to write to</returns>
        public static int CalibrationDrawPoint(float x, float y, Sample[] buffer, int numSamples, int startIndex)
        {
            int finalIndexPlusOne = startIndex + numSamples;
            for (; startIndex < finalIndexPlusOne; startIndex++)
            {
                buffer[startIndex] = new Sample(x, y);
            }
            return finalIndexPlusOne;
        }
    }
}
