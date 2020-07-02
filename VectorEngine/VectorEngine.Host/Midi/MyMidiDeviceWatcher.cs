using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace VectorEngine.Host.Midi
{
    // Helper class copied from here: https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/midi
    public class MyMidiDeviceWatcher
    {
        DeviceWatcher deviceWatcher;
        string deviceSelectorString;

        public DeviceInformationCollection DeviceInformationCollection { get; set; }

        public Action OnEnumerationComplete = null;

        public MyMidiDeviceWatcher(string midiDeviceSelectorString, Action onEnumerationComplete)
        {
            deviceSelectorString = midiDeviceSelectorString;
            OnEnumerationComplete += onEnumerationComplete;

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
            await Task.Run(() => UpdateDevices());
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            await Task.Run(() => UpdateDevices());
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            await Task.Run(() => UpdateDevices());
            OnEnumerationComplete?.Invoke();
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await Task.Run( () => UpdateDevices());
        }

        private async Task UpdateDevices()
        {
            // Get a list of all MIDI devices
            this.DeviceInformationCollection = await DeviceInformation.FindAllAsync(deviceSelectorString);
        }
    }
}
