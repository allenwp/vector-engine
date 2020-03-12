using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BlueWave.Interop.Asio;
using VectorEngine.Engine;

namespace VectorEngine.Output
{
    public class ASIOOutput
    {
		public static void StartDriver()
		{
			// TODO:This call to highest priority probably doesn't mean anything at all
			// because it's a different thread that that handles the events...
			// Unless... this thread will determine the other thread's priority based on its own??
			// no messing, this is high priority stuff
			Thread.CurrentThread.Priority = ThreadPriority.Highest;

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
				Console.WriteLine(string.Format("  {0}. {1}", index + 1, AsioDriver.InstalledDrivers[index]));
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
			driver.ShowControlPanel();

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

		/// <summary>
		/// Called when a buffer update is required
		/// </summary>
		private static void AsioDriver_BufferUpdate(object sender, EventArgs e)
		{
			// the driver is the sender
			AsioDriver driver = sender as AsioDriver;

			// get the stereo output channels
			Channel xOutput = driver.OutputChannels[0];
			Channel yOutput = driver.OutputChannels[1];
			Channel zOutput = driver.OutputChannels[2];

			FeedAsioBuffers(xOutput, yOutput, zOutput, 0);
		}

		static int frameIndex = 0;
		private static void FeedAsioBuffers(Channel leftOutput, Channel rightOutput, Channel brightnessOutput, int startIndex)
		{
			if ((FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.WaitingToReadBuffer1 && FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WrittingBuffer1)
				|| (FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.WaitingToReadBuffer2 && FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WrittingBuffer2))
			{
				int blankedSampleCount = leftOutput.BufferSize - startIndex;
				FrameOutput.StarvedSamples += blankedSampleCount;
				Console.WriteLine("AUDIO BUFFER IS STARVED FOR FRAMES! Blanking for " + blankedSampleCount);
				// Clear the rest of the buffer with blanking frames
				for (int i = startIndex; i < leftOutput.BufferSize; i++)
				{
					leftOutput[i] = -1f;
					rightOutput[i] = -1f;
					brightnessOutput[i] = 0f;
				}
				return;
			}

			Sample[] currentFrameBuffer = null;
			var readState = (FrameOutput.ReadStateEnum)FrameOutput.ReadState;
			switch (readState)
			{
				case FrameOutput.ReadStateEnum.WaitingToReadBuffer1:
					FrameOutput.ReadState = (int)FrameOutput.ReadStateEnum.ReadingBuffer1;
					currentFrameBuffer = FrameOutput.Buffer1;
					break;
				case FrameOutput.ReadStateEnum.ReadingBuffer1:
					currentFrameBuffer = FrameOutput.Buffer1;
					break;
				case FrameOutput.ReadStateEnum.WaitingToReadBuffer2:
					FrameOutput.ReadState = (int)FrameOutput.ReadStateEnum.ReadingBuffer2;
					currentFrameBuffer = FrameOutput.Buffer2;
					break;
				case FrameOutput.ReadStateEnum.ReadingBuffer2:
					currentFrameBuffer = FrameOutput.Buffer2;
					break;
			}

			for (int i = startIndex; i < leftOutput.BufferSize; i++)
			{
				// Move to the next buffer if needed by recursively calling this method:
				if (frameIndex >= currentFrameBuffer.Length)
				{
					readState = (FrameOutput.ReadStateEnum)FrameOutput.ReadState;
					switch (readState)
					{
						case FrameOutput.ReadStateEnum.ReadingBuffer1:
							FrameOutput.ReadState = (int)FrameOutput.ReadStateEnum.WaitingToReadBuffer2;
							break;
						case FrameOutput.ReadStateEnum.ReadingBuffer2:
							FrameOutput.ReadState = (int)FrameOutput.ReadStateEnum.WaitingToReadBuffer1;
							break;
						case FrameOutput.ReadStateEnum.WaitingToReadBuffer1:
						case FrameOutput.ReadStateEnum.WaitingToReadBuffer2:
						default:
							throw new Exception("We should be in the middle of reading a buffer, but for some reason the state says we're waiting.");
					}

					frameIndex = 0;
					FeedAsioBuffers(leftOutput, rightOutput, brightnessOutput, i);
					return;
				}

				Sample sample = PrepareSampleForScreen(currentFrameBuffer[frameIndex]);
				leftOutput[i] = sample.X;
				rightOutput[i] = sample.Y;
				brightnessOutput[i] = sample.Brightness;

				frameIndex++;
			}
		}

		public static Sample PrepareSampleForScreen(Sample sample)
		{
			if (FrameOutput.AspectRatio > 1f)
			{
				sample.X /= FrameOutput.AspectRatio;
				sample.Y /= FrameOutput.AspectRatio;
			}
			else
			{
				// Nothing to do with a portrait or square aspect ratio
				// In these cases, Y is already in range of -1 to 1 and
				// X is whatever range it needs to be to match the aspect ratio.
			}

			// TODO: adjust brightness to match voltage needed for z-input on oscilloscope.

			return sample;
		}
	}
}
