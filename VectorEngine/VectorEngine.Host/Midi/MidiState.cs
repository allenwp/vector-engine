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
        public enum MidiControlDescriptionType { Button, Knob }
        public struct MidiControlDescription
        {
            public byte Id;
            public MidiControlDescriptionType Type;

            public override string ToString()
            {
                return Type.ToString() + ": " + Id.ToString();
            }
        }

        /// <summary>
        /// When true the editor should be ready to accept a new assignment for the AssigningControl
        /// </summary>
        public bool Assigning { get; private set; } = false;

        public MidiControlDescriptionType LastAssignmentType { get; private set; }

        /// <summary>
        /// The assignment button that was last pressed.
        /// </summary>
        private byte lastAssignmentButton = 0;

        /// <summary>
        /// byte is the assign code rather than the actual button / knob
        /// </summary>
        public Dictionary<byte, MidiControlState> ControlStates { get; private set; } = new Dictionary<byte, MidiControlState>();
        /// <summary>
        /// Assign: the button that should be pressed to start/end assignemnt of a control.
        /// Control: the control button or knob
        /// </summary>
        public Dictionary<byte, MidiControlDescription> AssignToControlMapping { get; private set; } = new Dictionary<byte, MidiControlDescription>();

        public MidiState()
        {
            // Knobs
            for (int i = 0; i < 8; i++)
            {
                AssignToControlMapping[(byte)i] = new MidiControlDescription() { Id = (byte)(i + 1), Type = MidiControlDescriptionType.Knob };
            }
            for (int i = 24; i < 32; i++)
            {
                AssignToControlMapping[(byte)i] = new MidiControlDescription() { Id = (byte)(i - 13), Type = MidiControlDescriptionType.Knob };
            }

            // Buttons
            for (int i = 16; i < 24; i++)
            {
                AssignToControlMapping[(byte)i] = new MidiControlDescription() { Id = (byte)(i - 8), Type = MidiControlDescriptionType.Button };
            }
            for (int i = 40; i < 48; i++)
            {
                AssignToControlMapping[(byte)i] = new MidiControlDescription() { Id = (byte)(i - 8), Type = MidiControlDescriptionType.Button };
            }

            foreach (var pair in AssignToControlMapping)
            {
                ControlStates[pair.Key] = new MidiControlState();
            }

            foreach (var pair in AssignToControlMapping)
            {
                if (AssignToControlMapping.Where(checkPair => checkPair.Value.Id == pair.Value.Id && checkPair.Value.Type == pair.Value.Type).Count() > 1)
                {
                    throw new Exception("MidiState: Two controls share the same Id and Type!");
                }
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
                    lastAssignmentButton = buttonNumber;
                    LastAssignmentType = AssignToControlMapping[buttonNumber].Type;
                    if (Assigning)
                    {
                        // Clear out the current assignment if we're starting a new assignment.
                        var controlState = ControlStates[buttonNumber];
                        controlState.ControlledObject = null;
                        controlState.FieldPropertyInfo = null;
                        ControlStates[buttonNumber] = controlState;
                    }
                }
                else
                {
                    var collection = AssignToControlMapping.Where(pair => pair.Value.Id == buttonNumber && pair.Value.Type == MidiControlDescriptionType.Button);
                    if (collection.Count() > 0)
                    {
                        // We pressed a button control
                        byte assignmentCode = collection.FirstOrDefault().Key;
                        var controlState = ControlStates[assignmentCode];
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
            }

            if (midiMessage.Type == MidiMessageType.ControlChange)
            {
                var message = (MidiControlChangeMessage)midiMessage;
                int delta = message.ControlValue - MIDI.KNOB_CENTER;
                Console.WriteLine("MIDI: Turned knob " + message.Controller + " with a delta of " + delta);

                var collection = AssignToControlMapping.Where(pair => pair.Value.Id == message.Controller && pair.Value.Type == MidiControlDescriptionType.Knob);
                if (collection.Count() > 0)
                {
                    byte assignmentCode = collection.FirstOrDefault().Key;
                    var controlState = ControlStates[assignmentCode];
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

        public void AssignControl(object controlledObject, FieldPropertyInfo fieldPropertyInfo)
        {
            AssignControl(controlledObject, fieldPropertyInfo, lastAssignmentButton);
        }

        public void AssignControl(object controlledObject, FieldPropertyInfo fieldPropertyInfo, byte assignmentButton)
        {
            if (ControlStates.ContainsKey(assignmentButton))
            {
                var controlState = ControlStates[assignmentButton];
                controlState.ControlledObject = controlledObject;
                controlState.FieldPropertyInfo = fieldPropertyInfo;
                ControlStates[assignmentButton] = controlState;
            }

            Assigning = false;
        }
    }
}
