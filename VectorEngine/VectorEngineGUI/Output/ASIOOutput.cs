using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BlueWave.Interop.Asio;

namespace VectorEngine.Output
{
    public class ASIOOutput
    {
		public static void ThreadMethod()
		{
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
			int driverNumber = 1;

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

			while(true)
			{
				Thread.Sleep(100);
			}

			// and all donw
			driver.Stop();
		}

		static double t = 0;
		/// <summary>
		/// Called when a buffer update is required
		/// </summary>
		private static void AsioDriver_BufferUpdate(object sender, EventArgs e)
		{
			// the driver is the sender
			AsioDriver driver = sender as AsioDriver;

			// get the stereo output channels
			Channel leftOutput = driver.OutputChannels[0];
			Channel rightOutput = driver.OutputChannels[1];

			for (int index = 0; index < leftOutput.BufferSize; index++)
			{
				t += 0.03;
				leftOutput[index] = (float)Math.Sin(t);
				rightOutput[index] = (float)Math.Cos(t);
			}
		}
	}
}
