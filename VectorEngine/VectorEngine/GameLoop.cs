using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using VectorEngine.Output;

namespace VectorEngine
{
    public class GameLoop
    {
        #region Performance
        // Some performance counters
        static PerfTime syncOverheadTime = PerfTime.Initial;
        static PerfTime frameTimePerf = PerfTime.Initial;
        static PerfTime hostTimePerf = PerfTime.Initial;
        static Stopwatch swFrameSyncOverhead = new Stopwatch();
        static Stopwatch swHostTime = new Stopwatch();
        #endregion

        /// <summary>
        /// Used for determining how blanking should behave for the first sample of a new frame.
        /// </summary>
        static Sample previousFinalSample = Sample.Blank;

        static bool firstFrame = true;

        /// <summary>
        /// Set to false to pause ALL systems, regardless of whether it's in editor or game mode.
        /// </summary>
        public static bool TickSystems = true;

        #region Double Frame Buffer
        enum WriteStateEnum
        {
            Buffer1,
            Buffer2
        }
        static WriteStateEnum WriteState = WriteStateEnum.Buffer1;
        #endregion

        public static void Init(List<ECSSystem> systems, List<Component> components)
        {
            // ASIO or other output should be the highest priority thread so that it can
            // at least feed blanking samples to the screen if the game loop doesn't finish
            // rendering in time. The game loop is one priority lower, but still above normal.
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            EntityAdmin.Instance.Init(systems, components);
        }

        public static void Tick()
        {
            swHostTime.Stop();
            PerfTime.RecordPerfTime(swHostTime, ref hostTimePerf);
            swFrameSyncOverhead.Start();

            WriteStateEnum writeState = (WriteStateEnum)WriteState;
            // If we're still waiting for the output to finish reading a buffer, don't do anything else
            if (!firstFrame
                && ((writeState == WriteStateEnum.Buffer1 && FrameOutput.Buffer1 != null)
                    || (writeState == WriteStateEnum.Buffer2 && FrameOutput.Buffer2 != null)))
            {
                // TODO: do something better with thread locks or Parallel library or something that doesn't involve spinning??
                return;
            }

            swFrameSyncOverhead.Stop();
            PerfTime.RecordPerfTime(swFrameSyncOverhead, ref syncOverheadTime);
            var swFrameTime = new Stopwatch();
            swFrameTime.Start();

            // Add first because some components in the Add queue might also be queued for removal.
            // (For example, when adding a component, but then clearing a scene later in the same frame.)
            // But it doesn't make sense that a component would be first removed and then added.
            EntityAdmin.Instance.AddQueuedComponents();
            EntityAdmin.Instance.RemoveQueuedComponents();

            // Tick the systems
            if (TickSystems)
            {
                foreach (ECSSystem system in EntityAdmin.Instance.Systems)
                {
#if !DEBUG
                    try
                    {
#endif
                        system.Tick();
#if !DEBUG
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
#endif
                 }
            }

            // Finally, prepare and fill the FrameOutput buffer:
            int blankingSampleCount;
            int wastedSampleCount;
            Sample[] finalBuffer = CreateFrameBuffer(EntityAdmin.Instance.GetComponents<SamplerSingleton>().First().LastSamples, previousFinalSample, out blankingSampleCount, out wastedSampleCount); // FrameOutput.GetCalibrationFrame();
            previousFinalSample = finalBuffer[finalBuffer.Length - 1];

            // Debug test code to simulate tricky double buffer situations
            //if (new Random().Next(60) == 0)
            //{
            //    //Console.WriteLine("Sleeping to simulate a long frame time.");
            //    Thread.Sleep(7);
            //}

            swFrameTime.Stop();
            PerfTime.RecordPerfTime(swFrameTime, ref frameTimePerf);
            swFrameSyncOverhead.Reset();
            swFrameSyncOverhead.Start();

            // "Blit" the buffer and progress the frame buffer write state
            if (writeState == WriteStateEnum.Buffer1)
            {
                FrameOutput.Buffer1 = finalBuffer;
                WriteState = WriteStateEnum.Buffer2;
            }
            else
            {
                FrameOutput.Buffer2 = finalBuffer;
                WriteState = WriteStateEnum.Buffer1;
            }

            // This part regarding the number of starved samples is not thread perfect, but I think it should be
            // correct more than 99.9% of the time...
            int starvedSamples = FrameOutput.StarvedSamples;
            FrameOutput.StarvedSamples = 0;

            // Update GameTime:
            GameTime.LastFrameSampleCount = finalBuffer.Length + starvedSamples;
            if (starvedSamples > 0)
            {
                Console.WriteLine("Added " + starvedSamples + " starved samples to GameTime.LastFrameSampleCount.");
            }

            FrameOutput.FrameCount++;

            if (firstFrame)
            {
                firstFrame = false;
                Console.WriteLine("Finished creating first frame buffer! Starting ASIOOutput now.");
                ASIOOutput.StartDriver();
            }

            if (FrameOutput.FrameCount % 100 == 0)
            {
                int frameRate = (int)Math.Round(1 / ((float)GameTime.LastFrameSampleCount / FrameOutput.SAMPLES_PER_SECOND));
                Console.WriteLine(" " + finalBuffer.Length + " + " + starvedSamples + " starved samples = " + frameRate + " fps (" + blankingSampleCount + " blanking between shapes, " + wastedSampleCount + " wasted) | Frame worst: " + frameTimePerf.worst + " best: " + frameTimePerf.best + " avg: " + frameTimePerf.average + " | Output Sync longest: " + syncOverheadTime.worst + " shortest: " + syncOverheadTime.best + " avg: " + syncOverheadTime.average + " | Host worst: " + hostTimePerf.worst + " best: " + hostTimePerf.best + " avg: " + hostTimePerf.average);
                frameTimePerf = PerfTime.Initial;
                syncOverheadTime = PerfTime.Initial;
                hostTimePerf = PerfTime.Initial;
            }
            swFrameSyncOverhead.Stop();
            swHostTime.Reset();
            swHostTime.Start();
        }

        delegate void AddSamplesDelegate(Sample[] sampleArray);

        /// <param name="previousFrameEndSample">The sample that was drawn right before starting to draw this frame. (Last sample from the previous frame)</param>
        private static Sample[] CreateFrameBuffer(List<Sample[]> samples, Sample previousFrameEndSample, out int blankingSamples, out int wastedSamples)
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
            int worstCaseBlankingLength = FrameOutput.DisplayProfile.BlankingLength(new Sample(-1f, -1f, 0), new Sample(1f, 1f, 0));
            int worstCaseSampleCount = sampleCount + (samples.Count * worstCaseBlankingLength) + worstCaseBlankingLength + 1; // one more on the end to return to blanking point.

            Sample[] finalBuffer = new Sample[worstCaseSampleCount];

            // Copy the full set of samples into the final buffer:
            int destinationIndex = 0;
            Sample previousSample = previousFrameEndSample;

            AddSamplesDelegate addSamples = delegate(Sample[] sampleArray)
            {
                int blankingLength = FrameOutput.DisplayProfile.BlankingLength(previousSample, sampleArray[0]);
                // Set blanking based on the first sample:
                for (int b = 0; b < blankingLength; b++)
                {
                    Sample tweenSample = new Sample();
                    float tweenValue = Tween.EaseInOutPower((b + 1) / (float)blankingLength, 2);
                    tweenSample.X = MathHelper.Lerp(previousSample.X, sampleArray[0].X, tweenValue);
                    tweenSample.Y = MathHelper.Lerp(previousSample.Y, sampleArray[0].Y, tweenValue);

                    finalBuffer[destinationIndex] = tweenSample;
                    finalBuffer[destinationIndex].Brightness = 0f;
                    destinationIndex++;
                }

                // Then copy the samples over:
                Array.Copy(sampleArray, 0, finalBuffer, destinationIndex, sampleArray.Length);
                destinationIndex += sampleArray.Length;
                previousSample = sampleArray[sampleArray.Length - 1];
            };

            foreach (var sampleArray in samples)
            {
                addSamples(sampleArray);
            }

            int finalSampleCount = destinationIndex;
            if (finalSampleCount < FrameOutput.TARGET_BUFFER_SIZE)
            {
                // Since we're going to be blanking for the end of this frame, we need to do the same easeinout blanking before we get to the rest postition.
                addSamples(new Sample[1] { Sample.Blank });
                finalSampleCount = destinationIndex;
            }

            blankingSamples = finalSampleCount - sampleCount;

            Sample[] trimmedFinalBuffer;
            // Set up the final buffer with the correct sample length after dynamic blanking has been performed
            // This is variable (variable frame rate based on paramenters in FrameOutput class)
            if (finalSampleCount < FrameOutput.TARGET_BUFFER_SIZE)
            {
                trimmedFinalBuffer = new Sample[FrameOutput.TARGET_BUFFER_SIZE];
                // Only in this case to we need to clear the last bits of the buffer.
                // In the other cases we will be filling it entirely
                FrameOutput.ClearBuffer(trimmedFinalBuffer, finalSampleCount);
            }
            else
            {
                trimmedFinalBuffer = new Sample[finalSampleCount];
            }
            Array.Copy(finalBuffer, 0, trimmedFinalBuffer, 0, finalSampleCount);

            wastedSamples = trimmedFinalBuffer.Length - finalSampleCount;

            return trimmedFinalBuffer;
        }
    }
}
