using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi; // Needs Microsoft.Windows.SDK.Contracts NuGet Package as described here: https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance

namespace VectorEngineWPFGUI
{
    public class MIDI
    {
        MyMidiDeviceWatcher inputDeviceWatcher;
        MyMidiDeviceWatcher outputDeviceWatcher;

        MidiInPort midiInPort;
        IMidiOutPort midiOutPort;

        public void SetupWatchers()
        {
            inputDeviceWatcher = new MyMidiDeviceWatcher(MidiInPort.GetDeviceSelector());
            inputDeviceWatcher.StartWatcher();

            outputDeviceWatcher = new MyMidiDeviceWatcher(MidiOutPort.GetDeviceSelector());
            outputDeviceWatcher.StartWatcher();
        }

        public void TeardownWatchers()
        {
            inputDeviceWatcher.StopWatcher();
            inputDeviceWatcher = null;

            outputDeviceWatcher.StopWatcher();
            outputDeviceWatcher = null;

            midiInPort.MessageReceived -= MidiInPort_MessageReceived;
            midiInPort.Dispose();
            midiInPort = null;

            midiOutPort.Dispose();
            midiOutPort = null;
        }

        public async void SetupMidiPorts()
        {
            var inDeviceInformationCollection = inputDeviceWatcher.DeviceInformationCollection;
            var outDeviceInformationCollection = outputDeviceWatcher.DeviceInformationCollection;

            if (inDeviceInformationCollection == null || outDeviceInformationCollection == null)
            {
                return;
            }

            DeviceInformation inDevInfo = null;
            foreach (var info in inDeviceInformationCollection)
            {
                if (info.Name.Contains("X-TOUCH MINI"))
                {
                    inDevInfo = info;
                }
            }
            DeviceInformation outDevInfo = null;
            foreach (var info in outDeviceInformationCollection)
            {
                if (info.Name.Contains("X-TOUCH MINI"))
                {
                    outDevInfo = info;
                }
            }

            if (inDevInfo == null || outDevInfo == null)
            {
                return;
            }

            midiInPort = await MidiInPort.FromIdAsync(inDevInfo.Id);
            if (midiInPort == null)
            {
                Console.WriteLine("Unable to create MidiInPort from input device " + inDevInfo.Name);
                return;
            }
            midiInPort.MessageReceived += MidiInPort_MessageReceived;

            midiOutPort = await MidiOutPort.FromIdAsync(outDevInfo.Id);
            if (midiOutPort == null)
            {
                Console.WriteLine("Unable to create MidiOutPort from output device " + outDevInfo.Name);
                return;
            }
            for (byte controller = 0; controller < 18; controller++)
            {
                ResetKnob(controller);
            }
        }

        private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            IMidiMessage receivedMidiMessage = args.Message;

            if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
            {
                Console.WriteLine("Pressed button number: " + ((MidiNoteOnMessage)receivedMidiMessage).Note);
            }

            if (receivedMidiMessage.Type == MidiMessageType.ControlChange)
            {
                var message = (MidiControlChangeMessage)receivedMidiMessage;

                Console.WriteLine("Turned knob " + message.Controller + " to new value of " + message.ControlValue);

                ResetKnob(message.Controller);
            }
        }

        private void ResetKnob(byte controller)
        {
            byte channel = 10; // Seems to always be channel 10 on the X-Touch Mini? TODO: If it isn't, fix this.
            byte controlValue = 64;
            IMidiMessage midiMessageToSend = new MidiControlChangeMessage(channel, controller, controlValue);
            midiOutPort.SendMessage(midiMessageToSend);
        }
    }
}
