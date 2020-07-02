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
    public class MidiControlState
    {
        /// <summary>
        /// When true the editor should be ready to accept a new assignment for the AssigningControl
        /// </summary>
        public bool Assigning = false;
        /// <summary>
        /// The control button, not the assignment button.
        /// </summary>
        public byte AssigningControl = 0;

        /// <summary>
        /// Assign: the button that should be clicked to start/end assignment of a control knob.
        /// Control: the control knob
        /// </summary>
        Dictionary<byte, byte> KnobAssignToControlMapping = new Dictionary<byte, byte>();

        // TOOD: Dictionary<byte, byte> KnobResetScaleToScaleMapping = new Dictionary<byte, byte>();

        Dictionary<byte, ButtonControlState> ButtonControlStates = new Dictionary<byte, ButtonControlState>();
        /// <summary>
        /// Assign: the button that should be pressed to start/end assignemnt of a control button.
        /// Control: the control button
        /// </summary>
        Dictionary<byte, byte> ButtonAssignToControlMapping = new Dictionary<byte, byte>();

        public MidiControlState()
        {
            for (int i = 16; i < 24; i++)
            {
                ButtonAssignToControlMapping[(byte)i] = (byte)(i - 8);
            }
            for (int i = 40; i < 48; i++)
            {
                ButtonAssignToControlMapping[(byte)i] = (byte)(i - 8);
            }

            for (int i = 1; i < 8; i += 2)
            {
                KnobAssignToControlMapping[(byte)i] = (byte)(i + 1);
            }
            for (int i = 25; i < 32; i += 2)
            {
                KnobAssignToControlMapping[(byte)i] = (byte)(i - 13);
            }

            foreach (var pair in ButtonAssignToControlMapping)
            {
                ButtonControlStates[pair.Value] = new ButtonControlState();
            }
        }

        public void UpdateState(IMidiMessage midiMessage)
        {
            if (midiMessage.Type == MidiMessageType.NoteOn)
            {
                var buttonNumber = ((MidiNoteOnMessage)midiMessage).Note;
                Console.WriteLine("MIDI: Pressed button number: " + buttonNumber);
                if (ButtonAssignToControlMapping.ContainsKey(buttonNumber))
                {
                    // We pressed a button assignment button
                    Assigning = !Assigning;
                    AssigningControl = ButtonAssignToControlMapping[buttonNumber];
                    if (Assigning)
                    {
                        // Clear out the current assignment if we're starting a new assignment.
                        var controlState = ButtonControlStates[AssigningControl];
                        controlState.ControlledObject = null;
                        controlState.FieldPropertyInfo = null;
                        ButtonControlStates[AssigningControl] = controlState;
                    }
                }
                else if (ButtonControlStates.ContainsKey(buttonNumber))
                {
                    // We pressed a button control
                    var controlState = ButtonControlStates[buttonNumber];
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
            }
        }

        public void AssignControl(object controlledObject, FieldPropertyInfo rieldPropertyInfo)
        {
            if (ButtonControlStates.ContainsKey(AssigningControl))
            {
                var controlState = ButtonControlStates[AssigningControl];
                controlState.ControlledObject = controlledObject;
                controlState.FieldPropertyInfo = rieldPropertyInfo;
                ButtonControlStates[AssigningControl] = controlState;
            }

            Assigning = false;
        }
    }
}
