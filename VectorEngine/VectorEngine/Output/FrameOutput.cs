using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VectorEngine.Engine;
using VectorEngine.Engine.Util;

namespace VectorEngine.Output
{
    public class FrameOutput
    {
        public static readonly int SAMPLES_PER_SECOND = 192000;
        public static readonly float TARGET_FRAMES_PER_SECOND = 80f;
        public static readonly int TARGET_BUFFER_SIZE = (int)Math.Round(SAMPLES_PER_SECOND / TARGET_FRAMES_PER_SECOND);

        public static float AspectRatio = 4f / 3.5f;

        /// <summary>
        /// Number of samples for each blank at a distance of 1 unit between samples
        /// </summary>
        private static float blankingLength = 14f;
        public  static int BlankingLength(Sample sample1, Sample sample2)
        {
            return 25;
            // Clamp these because that's what's going to happen at output time anyway
            sample1.Clamp();
            sample2.Clamp();
            return (int)Math.Ceiling(blankingLength * SampleUtil.DistanceBetweenSamples(sample1, sample2));
        }

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
            WrittingBuffer1 = 0,
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

            bufferIndex = CalibrationDrawPoint(-AspectRatio, -1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(-AspectRatio, 0, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(-AspectRatio, 1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(0, 1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(AspectRatio, 1, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(AspectRatio, 0, buffer, brightestSampleCount, bufferIndex);
            bufferIndex = CalibrationDrawPoint(AspectRatio, -1, buffer, brightestSampleCount, bufferIndex);
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
