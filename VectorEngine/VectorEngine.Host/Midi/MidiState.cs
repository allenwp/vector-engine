using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Host.Reflection;
using Windows.Devices.Midi;
using Windows.UI.Xaml.Controls;

namespace VectorEngine.Host.Midi
{
    public class MidiState
    {
        /// <summary>
        /// When true the editor should be ready to accept a new assignment for the AssigningControl
        /// </summary>
        public bool Assigning = false;
        /// <summary>
        /// The control button or knob, not the assignment button.
        /// </summary>
        public byte AssigningControl = 0;

        // TOOD: Dictionary<byte, byte> KnobResetScaleToScaleMapping = new Dictionary<byte, byte>();

        Dictionary<byte, MidiControlState> ControlStates = new Dictionary<byte, MidiControlState>();
        /// <summary>
        /// Assign: the button that should be pressed to start/end assignemnt of a control.
        /// Control: the control button or knob
        /// </summary>
        Dictionary<byte, byte> AssignToControlMapping = new Dictionary<byte, byte>();

        public MidiState()
        {
            for (int i = 16; i < 24; i++)
            {
                AssignToControlMapping[(byte)i] = (byte)(i - 8);
            }
            for (int i = 40; i < 48; i++)
            {
                AssignToControlMapping[(byte)i] = (byte)(i - 8);
            }

            for (int i = 1; i < 8; i += 2)
            {
                AssignToControlMapping[(byte)i] = (byte)(i + 1);
            }
            for (int i = 25; i < 32; i += 2)
            {
                AssignToControlMapping[(byte)i] = (byte)(i - 13);
            }

            foreach (var pair in AssignToControlMapping)
            {
                ControlStates[pair.Value] = new MidiControlState();
            }
        }

        public void UpdateState(IMidiMessage midiMessage)
        {
            if (midiMessage.Type == MidiMessageType.NoteOn)
            {
                var buttonNumber = ((MidiNoteOnMessage)midiMessage).Note;
                Console.WriteLine("MIDI: Pressed button number: " + buttonNumber);
                if (AssignToControlMapping.ContainsKey(buttonNumber))
                {
                    // We pressed a button assignment button
                    Assigning = !Assigning;
                    AssigningControl = AssignToControlMapping[buttonNumber];
                    if (Assigning)
                    {
                        // Clear out the current assignment if we're starting a new assignment.
                        var controlState = ControlStates[AssigningControl];
                        controlState.ControlledObject = null;
                        controlState.FieldPropertyInfo = null;
                        ControlStates[AssigningControl] = controlState;
                    }
                }
                else if (ControlStates.ContainsKey(buttonNumber))
                {
                    // We pressed a button control
                    var controlState = ControlStates[buttonNumber];
                    if (controlState.ControlledObject != null)
                    {
                        if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(bool))
                        {
                            bool val = (bool)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                            val = !val;
                            controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                        }
                    }
                }
            }

            if (midiMessage.Type == MidiMessageType.ControlChange)
            {
                var message = (MidiControlChangeMessage)midiMessage;
                int delta = message.ControlValue - MIDI.KNOB_CENTER;
                Console.WriteLine("MIDI: Turned knob " + message.Controller + " with a delta of " + delta);

                if (ControlStates.ContainsKey(message.Controller))
                {
                    var controlState = ControlStates[message.Controller];
                    if (controlState.ControlledObject != null)
                    {
                        if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(float))
                        {
                            float val = (float)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                            val += delta;
                            controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                        }
                        else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(int))
                        {
                            int val = (int)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                            val += delta;
                            controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                        }
                        else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(uint))
                        {
                            int val = (int)((uint)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject));
                            val += delta;
                            if (val < 0)
                            {
                                val = 0;
                            }
                            controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, (uint)val);
                        }
                    }
                }
            }
        }

        public void AssignControl(object controlledObject, FieldPropertyInfo rieldPropertyInfo)
        {
            if (ControlStates.ContainsKey(AssigningControl))
            {
                var controlState = ControlStates[AssigningControl];
                controlState.ControlledObject = controlledObject;
                controlState.FieldPropertyInfo = rieldPropertyInfo;
                ControlStates[AssigningControl] = controlState;
            }

            Assigning = false;
        }
    }
}
