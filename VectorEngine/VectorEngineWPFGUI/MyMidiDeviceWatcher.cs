using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace VectorEngineWPFGUI
{
    // Helper class copied from here: https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/midi
    public class MyMidiDeviceWatcher
    {
        DeviceWatcher deviceWatcher;
        string deviceSelectorString;

        public DeviceInformationCollection DeviceInformationCollection { get; set; }

        public MyMidiDeviceWatcher(string midiDeviceSelectorString)
        {
            deviceSelectorString = midiDeviceSelectorString;

            deviceWatcher = DeviceInformation.CreateWatcher(deviceSelectorString);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        ~MyMidiDeviceWatcher()
        {
            deviceWatcher.Added -= DeviceWatcher_Added;
            deviceWatcher.Removed -= DeviceWatcher_Removed;
            deviceWatcher.Updated -= DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
            deviceWatcher = null;
        }

        public void StartWatcher()
        {
            deviceWatcher.Start();
        }
        public void StopWatcher()
        {
            deviceWatcher.Stop();
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Update the device list
            UpdateDevices();
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            // Update the device list
            UpdateDevices();
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // Update the device list
            UpdateDevices();
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Update the device list
            UpdateDevices();
        }

        private async void UpdateDevices()
        {
            // Get a list of all MIDI devices
            this.DeviceInformationCollection = await DeviceInformation.FindAllAsync(deviceSelectorString);


            if (!this.DeviceInformationCollection.Any())
            {
                //deviceListBox.Items.Add("No MIDI devices found!");
            }

            foreach (var deviceInformation in this.DeviceInformationCollection)
            {
                //deviceListBox.Items.Add(deviceInformation.Name);
            }
        }
    }
}
