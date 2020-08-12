using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BlueWave.Interop.Asio;
using Microsoft.Xna.Framework;
using VectorEngine;

namespace VectorEngine.Output
{
    public class ASIOOutput
    {
        private static float[] blankingChannelDelayBuffer = new float[FrameOutput.BLANKING_CHANNEL_DELAY];

        public enum ReadStateEnum
        {
            Buffer1,
            Buffer2
        }
        public static ReadStateEnum ReadState = ReadStateEnum.Buffer1;

        public static void StartDriver()
        {
            // This apartment state is required for the ASIOOutput.StartDriver method
            // If an execption is thrown here that's because you need [STAThread] attribute on your static void Main(string[] args) method
            // or you need to set this thread's ApartmentState to STA before starting the thread.
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

            // make sure we have at least one ASIO driver installed
            if (AsioDriver.InstalledDrivers.Length == 0)
            {
                Console.WriteLine("There appears to be no ASIO drivers installed on your system.");
                Console.WriteLine("If your soundcard supports ASIO natively, install the driver");
                Console.WriteLine("from the support disc. If your soundcard has no native ASIO support");
                Console.WriteLine("you can probably use the generic ASIO4ALL driver.");
                Console.WriteLine("You can download this from: http://www.asio4all.com/");
                Console.WriteLine("It's very good!");
                Console.WriteLine();
                Console.WriteLine("Hit Enter to exit...");
                Console.ReadLine();
                return;
            }

            // bingo, we've go at least one
            Console.WriteLine("Your system has the following ASIO drivers installed:");
            Console.WriteLine();

            // so iterate through them
            for (int index = 0; index < AsioDriver.InstalledDrivers.Length; index++)
            {
                // and display them
                Console.WriteLine($"  {index + 1}. {AsioDriver.InstalledDrivers[index]}");
            }

            Console.WriteLine();

            // Currently hardcoded: todo: make it not.
            int driverNumber = 2;

            Console.WriteLine();
            Console.WriteLine("Using: " + AsioDriver.InstalledDrivers[driverNumber - 1]);
            Console.WriteLine();

            // load and activate the desited driver
            AsioDriver driver = AsioDriver.SelectDriver(AsioDriver.InstalledDrivers[driverNumber - 1]);

            // popup the driver's control panel for configuration
            //driver.ShowControlPanel();

            // now dump some details
            Console.WriteLine("  Driver name = " + driver.DriverName);
            Console.WriteLine("  Driver version = " + driver.Version);
            Console.WriteLine("  Input channels = " + driver.NumberInputChannels);
            Console.WriteLine("  Output channels = " + driver.NumberOutputChannels);
            Console.WriteLine("  Min buffer size = " + driver.BufferSizex.MinSize);
            Console.WriteLine("  Max buffer size = " + driver.BufferSizex.MaxSize);
            Console.WriteLine("  Preferred buffer size = " + driver.BufferSizex.PreferredSize);
            Console.WriteLine("  Granularity = " + driver.BufferSizex.Granularity);
            Console.WriteLine("  Sample rate = " + driver.SampleRate);

            if (driver.SampleRate != FrameOutput.SAMPLES_PER_SECOND)
            {
                throw new Exception("Driver sample rate is different than what the game expects. Please adjust driver settings to have a " + FrameOutput.SAMPLES_PER_SECOND + " sample rate.");
            }

            // get our driver wrapper to create its buffers
            driver.CreateBuffers(false);

            // write out the input channels
            Console.WriteLine("  Input channels found = " + driver.InputChannels.Length);
            Console.WriteLine("  ----");

            foreach (Channel channel in driver.InputChannels)
            {
                Console.WriteLine(channel.Name);
            }

            // and the output channels
            Console.WriteLine("  Output channels found = " + driver.OutputChannels.Length);
            Console.WriteLine("----");

            foreach (Channel channel in driver.OutputChannels)
            {
                Console.WriteLine(channel.Name);
            }

            // this is our buffer fill event we need to respond to
            driver.BufferUpdate += new EventHandler(AsioDriver_BufferUpdate);

            // and off we go
            driver.Start();

            //while(true)
            //{
            //	Thread.Sleep(100);
            //}

            //// and all donw
            //driver.Stop();
        }

        //static double t = 0;
        //static ulong frameCount = 0;
        //static bool high = false;
        /// <summary>
        /// Called when a buffer update is required
        /// </summary>
        private static void AsioDriver_BufferUpdate(object sender, EventArgs e)
        {
            // the driver is the sender
            AsioDriver driver = sender as AsioDriver;

            // get the stereo output channels
            Channel xOutput = driver.OutputChannels[2];
            Channel yOutput = driver.OutputChannels[3];
            Channel zOutput = driver.OutputChannels[0];

            FeedAsioBuffers(xOutput, yOutput, zOutput, 0);

            ApplyBlankingChannelDelay(zOutput);

            // Copy x output into the last of the four channels to give a demo of what the audio sounds like
            Channel audioDemo = driver.OutputChannels[1];
            for (int i = 0; i < xOutput.BufferSize; i++)
            {
                audioDemo[i] = xOutput[i];
            }

            ////Disable brightness modulation(z - input on scope)
            //for (int i = 0; i < zOutput.BufferSize; i++)
            //{
            //    zOutput[i] = 0;
            //}

            #region Debugging code
            // Code for a test tone to make sure ASIO device is working well:
            //for (int index = 0; index < xOutput.BufferSize; index++)
            //{
            //	t += 0.03;
            //	xOutput[index] = (float)Math.Sin(t);
            //	yOutput[index] = (float)Math.Sin(t);
            //	zOutput[index] = (float)Math.Sin(t);
            //}

            // Code for testing blanking:
            //for (int index = 0; index < xOutput.BufferSize; index++)
            //{
            //	t = Microsoft.Xna.Framework.MathHelper.Lerp(0, (float)Math.PI, (float)index / xOutput.BufferSize);
            //	xOutput[index] = (float)Math.Sin(t);
            //	yOutput[index] = (float)Math.Cos(t);

            //	if (yOutput[index] > 0.5f || yOutput[index] < -0.5f)
            //	{
            //		zOutput[index] = 1f;// (float)Math.Sin(t);
            //	}
            //	else
            //	{
            //		zOutput[index] = 0f;// (float)Math.Sin(t);
            //	}
            //}

            // Code for debugging flicker to 0,0:
            //for (int i = 0; i < xOutput.BufferSize; i++)
            //{
            //	double threshold = 0.01;
            //	if (Math.Abs((double)xOutput[i]) < threshold && Math.Abs((double)yOutput[i]) < threshold)
            //	{
            //		Console.WriteLine("output is 0");
            //	}
            //}

            //// Code for debugging oscilloscope faulty(?) X input that looks like it's not properly DC Coupled
            //frameCount++;
            //if (frameCount % 300 == 0)
            //{
            //	high = !high;
            //}
            //for (int i = 0; i < xOutput.BufferSize; i++)
            //{
            //	float value;
            //	if (i < xOutput.BufferSize / 2)
            //	{
            //		value = high ? 0.65f : -0.65f;
            //	}
            //	else
            //	{
            //		value = high ? 0.1f : -0.1f;
            //	}
            //	xOutput[i] = value;
            //	yOutput[i] = value;
            //	zOutput[i] = 0f;
            //}

            //// Alternative code for debugging oscilloscope faulty(?) X input that looks like it's not properly DC Coupled
            //frameCount++;
            //if (frameCount % 50 == 0)
            //{
            //	high = !high;
            //}
            //for (int i = 0; i < xOutput.BufferSize; i++)
            //{
            //	float value;
            //	value = high ? 0.1f : -0.1f;
            //	xOutput[i] = value;
            //	yOutput[i] = value;
            //	zOutput[i] = 0f;
            //}

            //// Alternative code for debugging oscilloscope faulty(?) X input that looks like it's not properly DC Coupled
            //frameCount++;
            //if (frameCount % 50 == 0)
            //{
            //	high = !high;
            //}
            //for (int i = 0; i < xOutput.BufferSize; i++)
            //{
            //	float value = -0.5f;
            //	if (high && i > xOutput.BufferSize / 3 && i < (xOutput.BufferSize / 3) * 2)
            //	{
            //		value = 0.5f;
            //	}
            //	xOutput[i] = value;
            //	zOutput[i] = 0f;
            //}
            //for (int i = 0; i < yOutput.BufferSize; i++)
            //{
            //	yOutput[i] = (float)i / (yOutput.BufferSize - 1);
            //	yOutput[i] = yOutput[i] * 2f - 1f;
            //	zOutput[i] = 0f;
            //}
            #endregion
        }

        static int frameIndex = 0;
        private static void FeedAsioBuffers(Channel xOutput, Channel yOutput, Channel brightnessOutput, int startIndex)
        {
            Sample[] currentFrameBuffer = ReadState == ReadStateEnum.Buffer1 ? FrameOutput.Buffer1 : FrameOutput.Buffer2;

            if (currentFrameBuffer == null)
            {
                int blankedSampleCount = xOutput.BufferSize - startIndex;
                FrameOutput.StarvedSamples += blankedSampleCount;
                Console.WriteLine("AUDIO BUFFER IS STARVED FOR FRAMES! Blanking for " + blankedSampleCount);
                // Clear the rest of the buffer with blanking frames
                for (int i = startIndex; i < xOutput.BufferSize; i++)
                {
                    // TODO: this should probably just pause on the last position instead (which might be blanking position, but might not be)
                    xOutput[i] = -1f;
                    yOutput[i] = -1f;
                    brightnessOutput[i] = 1f; // no brightness is 1
                }
                return;
            }

            for (int i = startIndex; i < xOutput.BufferSize; i++)
            {
                // Move to the next buffer if needed by recursively calling this method:
                if (frameIndex >= currentFrameBuffer.Length)
                {
                    CompleteFrame();
                    FeedAsioBuffers(xOutput, yOutput, brightnessOutput, i);
                    return;
                }

                Sample adjustedSample = PrepareSampleForScreen(currentFrameBuffer[frameIndex]);
                xOutput[i] = adjustedSample.X;
                yOutput[i] = adjustedSample.Y;
                brightnessOutput[i] = adjustedSample.Brightness;

                frameIndex++;
            }

            if (frameIndex >= currentFrameBuffer.Length)
            {
                CompleteFrame();
            }
        }

        public static void CompleteFrame()
        {
            switch (ReadState)
            {
                case ReadStateEnum.Buffer1:
                    FrameOutput.Buffer1 = null;
                    ReadState = ReadStateEnum.Buffer2;
                    break;
                case ReadStateEnum.Buffer2:
                    FrameOutput.Buffer2 = null;
                    ReadState = ReadStateEnum.Buffer1;
                    break;
            }

            frameIndex = 0;
        }

        public static Sample PrepareSampleForScreen(Sample sample)
        {
            float aspectRatio = FrameOutput.DisplayProfile.AspectRatio;
            if (aspectRatio > 1f)
            {
                sample.X /= aspectRatio;
                sample.Y /= aspectRatio;
            }
            else
            {
                // Nothing to do with a portrait or square aspect ratio
                // In these cases, Y is already in range of -1 to 1 and
                // X is whatever range it needs to be to match the aspect ratio.
            }

            // -1 is full brightness, 1 is no brightness
            sample.Brightness = MathHelper.Clamp(sample.Brightness, 0f, 1f) * -2f + 1f;

            return sample;
        }

        private static void ApplyBlankingChannelDelay(Channel blankingChannel)
        {
            int bufferLength = blankingChannel.BufferSize;

            float[] originalStream = new float[bufferLength];
            for (int i = 0; i < bufferLength; i++)
            {
                originalStream[i] = blankingChannel[i];
            }
            for (int i = 0; i < bufferLength; i++)
            {
                if (i < blankingChannelDelayBuffer.Length)
                {
                    blankingChannel[i] = blankingChannelDelayBuffer[i];
                }
                else
                {
                    blankingChannel[i] = originalStream[i - blankingChannelDelayBuffer.Length];
                }
            }
            Array.Copy(originalStream, originalStream.Length - blankingChannelDelayBuffer.Length, blankingChannelDelayBuffer, 0, blankingChannelDelayBuffer.Length);
        }
    }
}
