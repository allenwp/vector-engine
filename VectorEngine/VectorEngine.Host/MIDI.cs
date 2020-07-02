using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi; // Needs Microsoft.Windows.SDK.Contracts NuGet Package as described here: https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance

namespace VectorEngine.Host
{
    public class MIDI
    {
        public static readonly byte KNOB_CENTER = 64;

        MyMidiDeviceWatcher inputDeviceWatcher;
        MyMidiDeviceWatcher outputDeviceWatcher;

        MidiInPort midiInPort;
        IMidiOutPort midiOutPort;

        public bool SetupComplete = false;
        public ConcurrentQueue<IMidiMessage> MidiMessageQueue = new ConcurrentQueue<IMidiMessage>();

        public void SetupWatchersAndPorts()
        {
            Action enumerationInComplete = new Action(delegate () {

                Action enumerationOutComplete = new Action(delegate () {
                    SetupMidiPorts();
                });

                outputDeviceWatcher = new MyMidiDeviceWatcher(MidiOutPort.GetDeviceSelector(), enumerationOutComplete);
                outputDeviceWatcher.StartWatcher();
            });

            inputDeviceWatcher = new MyMidiDeviceWatcher(MidiInPort.GetDeviceSelector(), enumerationInComplete);
            inputDeviceWatcher.StartWatcher();
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
                Console.WriteLine("Could not find any MIDI devices.");
                SetupComplete = true;
                return;
            }

            DeviceInformation inDevInfo = null;
            string midiControllerName = "X-TOUCH MINI";
            foreach (var info in inDeviceInformationCollection)
            {
                if (info.Name.Contains(midiControllerName))
                {
                    inDevInfo = info;
                }
            }
            DeviceInformation outDevInfo = null;
            foreach (var info in outDeviceInformationCollection)
            {
                if (info.Name.Contains(midiControllerName))
                {
                    outDevInfo = info;
                }
            }

            if (inDevInfo == null || outDevInfo == null)
            {
                Console.WriteLine("Could not find a MIDI device with name: " + midiControllerName);
                SetupComplete = true;
                return;
            }

            midiInPort = await MidiInPort.FromIdAsync(inDevInfo.Id);
            if (midiInPort == null)
            {
                Console.WriteLine("Unable to create MidiInPort from input device " + inDevInfo.Name);
                SetupComplete = true;
                return;
            }
            midiInPort.MessageReceived += MidiInPort_MessageReceived;

            midiOutPort = await MidiOutPort.FromIdAsync(outDevInfo.Id);
            if (midiOutPort == null)
            {
                Console.WriteLine("Unable to create MidiOutPort from output device " + outDevInfo.Name);
                SetupComplete = true;
                return;
            }

            Console.WriteLine("Sucessfully found MIDI device with name: " + inDevInfo.Name);

            for (byte controller = 0; controller < 18; controller++)
            {
                ResetKnob(controller);
            }

            SetupComplete = true;
        }

        private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            IMidiMessage receivedMidiMessage = args.Message;
            if (receivedMidiMessage.Type == MidiMessageType.ControlChange)
            {
                var message = (MidiControlChangeMessage)receivedMidiMessage;
                ResetKnob(message.Controller);
            }
            
            MidiMessageQueue.Enqueue(receivedMidiMessage);
        }

        private void ResetKnob(byte controller)
        {
            byte channel = 10; // Seems to always be channel 10 on the X-Touch Mini? TODO: If it isn't, fix this.
            byte controlValue = KNOB_CENTER;
            IMidiMessage midiMessageToSend = new MidiControlChangeMessage(channel, controller, controlValue);
            midiOutPort.SendMessage(midiMessageToSend);
        }
    }
}
