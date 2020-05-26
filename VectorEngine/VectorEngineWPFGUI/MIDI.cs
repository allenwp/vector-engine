using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi; // Needs Microsoft.Windows.SDK.Contracts NuGet Package as described here: https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance

namespace VectorEngineWPFGUI
{
    public class MIDI
    {
        public static void GetDevices()
        {
            string midiInputQueryString = MidiInPort.GetDeviceSelector();
        }
    }
}
