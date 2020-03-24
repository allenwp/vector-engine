using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    public class GameLoop
    {
        static List<Shape> shapes = new List<Shape>();

        public static void Loop()
        {
            Init();

            while (true)
            {
                // If we're still waiting for the output to finish reading a buffer, don't do anything else
                if ((FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingToWriteBuffer1 && FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.ReadingBuffer1)
                    || (FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingToWriteBuffer2 && FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.ReadingBuffer2))
                {
                    // TODO: do something better with thread locks or Parallel library or something that doesn't involve spinning
                    continue;
                }

                // We're no longer waiting on output to finish with its buffer.
                // Progress the FrameOutput state
                FrameOutput.WriteStateEnum writeState;
                if (FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingToWriteBuffer1)
                {
                    writeState = FrameOutput.WriteStateEnum.WrittingBuffer1;
                }
                else
                {
                    writeState = FrameOutput.WriteStateEnum.WrittingBuffer2;
                }
                FrameOutput.WriteState = (int)writeState;

                // Tick the systems
                foreach (ECSSystem system in EntityAdmin.Instance.Systems)
                {
                    system.Tick();
                }

                // Finally, prepare and fill the FrameOutput buffer:
                Sample[] finalBuffer = CreateFrameBuffer(EntityAdmin.Instance.SingletonSampler.LastSamples); // FrameOutput.GetCalibrationFrame();

                // "Blit" the buffer and progress the frame buffer write state
                if (writeState == FrameOutput.WriteStateEnum.WrittingBuffer1)
                {
                    FrameOutput.Buffer1 = finalBuffer;
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WaitingToWriteBuffer2;
                }
                else
                {
                    FrameOutput.Buffer2 = finalBuffer;
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WaitingToWriteBuffer1;
                }

                // This part regarding the number of starved samples is not thread perfect, but I think it should be
                // correct more than 99.9% of the time...
                int starvedSamples = FrameOutput.StarvedSamples;
                FrameOutput.StarvedSamples = 0;

                // Update GameTime:
                GameTime.LastFrameSampleCount = finalBuffer.Length + starvedSamples;

                var oldFrameCount = FrameOutput.FrameCount; // to make sure we don't reinitialize when it overflows
                FrameOutput.FrameCount++;

                if (FrameOutput.FrameCount == 1 && oldFrameCount < FrameOutput.FrameCount)
                {
                    Console.WriteLine("Finished creating first frame buffer! Starting ASIOOutput now.");
                    ASIOOutput.StartDriver();
                }

                if (FrameOutput.FrameCount % 100 == 0)
                {
                    int frameRate = (int)Math.Round(1 / ((float)GameTime.LastFrameSampleCount / FrameOutput.SAMPLES_PER_SECOND));
                    Console.WriteLine(" Framerate: " + frameRate + "fps (" + finalBuffer.Length + " + " + starvedSamples + " starved samples)");
                }
            }
        }

        private static Sample[] CreateFrameBuffer(List<Sample[]> samples)
        {
            // Remove all empty sample arrays
            samples.RemoveAll(delegate (Sample[] array)
            {
                return array.Length < 1;
            });

            #region Sorting (Disabled code)

            // Sorting has the advantage of reducing beam overshooting between shapes.
            // It is disabled because it makes it worse! Here's why:
            // When shapes are sorted, they draw in a different order between frames.
            // This means that during one frame, a shape may be the first to be drawn
            // ...but on the next frame, they might be the last to be drawn.
            // In this case, it causes the shape to start flickering as if the refresh
            // rate is too low. I discovered this behavour immediately started happening
            // when I enabled this sorting method.
            // 
            // With this knowledge in mind, I actually think it is best to draw shapes in
            // the exact same order every frame, regardless of their position on the screen
            // in order to reduce flicker and give a smooth video. This means overshooting
            // will happen, but it's a less distracting problem than the flicker and jitter.

            //// Sort based on a starting point of Sample.Blank
            //List<Sample[]> sortedSamples = new List<Sample[]>(samples.Count);
            //Sample beamPosition = Sample.Blank;
            //while (samples.Count > 1)
            //{
            //    samples.Sort(delegate (Sample[] array1, Sample[] array2)
            //    {
            //        float distanceTo1 = DistanceBetweenSamples(beamPosition, array1[0]);
            //        float distanceTo2 = DistanceBetweenSamples(beamPosition, array2[0]);
            //        if (distanceTo1 < distanceTo2)
            //        {
            //            return -1;
            //        }
            //        else if (distanceTo1 > distanceTo2)
            //        {
            //            return 1;
            //        }
            //        else
            //        {
            //            return 0;
            //        }
            //    });
            //    beamPosition = samples[0][0];
            //    sortedSamples.Add(samples[0]);
            //    samples.Remove(samples[0]);
            //}
            //sortedSamples.Add(samples[0]);

            //samples = sortedSamples;
            #endregion

            // Find out how many samples we have in the full set
            int sampleCount = 0;
            foreach (var sampleArray in samples)
            {
                sampleCount += sampleArray.Length;
            }
            sampleCount += samples.Count * FrameOutput.BLANKING_LENGTH;

            Sample[] finalBuffer;
            // Set up the final buffer with the correct sample length
            // This is variable (variable frame rate based on paramenters in FrameOutput class)
            if (sampleCount <= FrameOutput.TARGET_BUFFER_SIZE)
            {
                finalBuffer = new Sample[FrameOutput.TARGET_BUFFER_SIZE];
                // Only in this case to we need to clear the buffer. In the other cases we will be filling it entirely
                FrameOutput.ClearBuffer(finalBuffer);
            }
            else
            {
                finalBuffer = new Sample[sampleCount];
            }

            // Copy the full set of samples into the final buffer:
            int destinationIndex = 0;
            foreach (var sampleArray in samples)
            {
                // Set blanking based on the first sample:
                for (int b = 0; b < FrameOutput.BLANKING_LENGTH; b++)
                {
                    finalBuffer[destinationIndex] = sampleArray[0];
                    finalBuffer[destinationIndex].Brightness = 0f;
                    destinationIndex++;
                }

                // Then copy the samples over:
                Array.Copy(sampleArray, 0, finalBuffer, destinationIndex, sampleArray.Length);
                destinationIndex += sampleArray.Length;
            }

            return finalBuffer;
        }

        private static float DistanceBetweenSamples(Sample sample1, Sample sample2)
        {
            return Vector2.Distance(new Vector2(sample1.X, sample1.Y), new Vector2(sample2.X, sample2.Y));
        }

        static void Init()
        {
            DemoGame.SceneSpaceRings.Init();
        }
    }
}
