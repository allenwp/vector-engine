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
        public static float TargetFramesPerSecond = 80f;
        public static int TargetBufferSize { get => (int)Math.Round(SAMPLES_PER_SECOND / TargetFramesPerSecond); }

        public static DisplayProfile DisplayProfile = new DisplayProfileOscTek2445();

        /// <summary>
        /// My PreSonus DAC has a delay on the channel I use for blanking -_-
        /// </summary>
        public static readonly int BLANKING_CHANNEL_DELAY = 17;

        #region Double Buffers
        /// <summary>
        /// GameLoop checks to see if it's null before writting.
        /// Output should set to null when it's finished reading.
        /// </summary>
        public static volatile Sample[] Buffer1;
        /// <summary>
        /// GameLoop checks to see if it's null before writting.
        /// Output should set to null when it's finished reading.
        /// </summary>
        public static volatile Sample[] Buffer2;
        #endregion

        public static volatile int StarvedSamples = 0;

        /// <summary>
        /// This will eventually overflow and loop back to 0.
        /// </summary>
        public static ulong FrameCount = 0;

        public static void ClearBuffer(Sample[] buffer, int startIndex = 0)
        {
            for(int i = startIndex; i < buffer.Length; i++)
            {
                buffer[i] = Sample.Blank;
            }
        }

        #region TODO: Move calibration stuff to a separate project
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
        #endregion
    }
}
