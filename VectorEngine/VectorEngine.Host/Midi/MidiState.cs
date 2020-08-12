using Xna = Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VectorEngine.Host.Reflection;
using Windows.Devices.Midi;

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

        public string LastAssignmentControlString
        {
            get
            {
                if (AssignToControlMapping.ContainsKey(lastAssignmentButton))
                {
                    MidiControlDescription desc = AssignToControlMapping[lastAssignmentButton];
                    return GetControlName(desc.Id, desc.Type);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// The assignment button that was last pressed.
        /// </summary>
        private byte lastAssignmentButton = 0;

        public float SliderValue { get; private set; } = 0.5f;

        /// <summary>
        /// Logarithmic lerp... TODO: Maybe move this to a math helper or something
        /// </summary>
        float logerp(float a, float b, float t)
        {
            return (float)(a * Math.Pow(b / a, t));
        }
        public float KnobControlStep
        {
            get
            {
                return logerp(0.01f, 100f, SliderValue) / 10f;
            }
        }

        /// <summary>
        /// byte is the assign code rather than the actual button / knob
        /// </summary>
        public Dictionary<byte, MidiControlState> ControlStates { get; private set; } = new Dictionary<byte, MidiControlState>();
        /// <summary>
        /// Assign: the button that should be pressed to start/end assignemnt of a control.
        /// Control: the control button or knob
        /// </summary>
        public Dictionary<byte, MidiControlDescription> AssignToControlMapping { get; private set; } = new Dictionary<byte, MidiControlDescription>();

        public string GetControlName(byte id, MidiControlDescriptionType type)
        {
            if (type == MidiControlDescriptionType.Knob)
            {
                if (id == 9 || id == 10)
                {
                    return $"Slider {(id == 9 ? 'A' : 'B')}";
                }
                else
                {
                    // 0 indexed layer and column
                    int layer = id < 9 ? 0 : 1;
                    int column;
                    if (layer < 1)
                    {
                        column = (id - 1) % 8;
                    }
                    else
                    {
                        column = (id - 11) % 8;
                    }
                    return $"Knob {(layer < 1 ? 'A' : 'B')}{column + 1}";
                }
            }
            else if (type == MidiControlDescriptionType.Button)
            {
                // 0 indexed layer, column, and row
                int layer = id < 24 ? 0 : 1;
                int column = id % 8;
                int row = (id / 8) - (layer > 0 ? 3 : 0);

                string rowString = row < 1 ? "" : (row < 2 ? " First Row" : " Second Row");
                return $"{(row < 1 ? "Knob Button" : "Button")} {(layer < 1 ? "A" : "B")}{column + 1}{rowString}";
            }
            return "";
        }

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

        public void ClearControlState(byte buttonNumber)
        {
            // Clear out the current assignment if we're starting a new assignment.
            var controlState = ControlStates[buttonNumber];
            controlState.ControlledObject = null;
            controlState.FieldPropertyInfo = null;
            ControlStates[buttonNumber] = controlState;
        }

        public void UpdateState(IMidiMessage midiMessage)
        {
            if (midiMessage.Type == MidiMessageType.NoteOn)
            {
                var buttonNumber = ((MidiNoteOnMessage)midiMessage).Note;
                Console.WriteLine("MIDI: Pressed " + GetControlName(buttonNumber, MidiControlDescriptionType.Button));
                if (AssignToControlMapping.ContainsKey(buttonNumber))
                {
                    // We pressed a button assignment button
                    Assigning = !Assigning;
                    lastAssignmentButton = buttonNumber;
                    LastAssignmentType = AssignToControlMapping[buttonNumber].Type;
                    if (Assigning)
                    {
                        ClearControlState(buttonNumber);
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
                            var info = controlState.FieldPropertyInfo;
                            if (info.FieldPropertyType == typeof(bool))
                            {
                                bool val = (bool)info.GetValue(controlState.ControlledObject);
                                val = !val;
                                info.SetValue(controlState.ControlledObject, val);
                            }
                            else if (info.FieldPropertyType.IsEnum)
                            {
                                var val = info.GetValue(controlState.ControlledObject);
                                var enumNames = info.FieldPropertyType.GetEnumNames();
                                int currentIndex = 0;
                                for (int i = 0; i < enumNames.Length; i++)
                                {
                                    if (enumNames[i] == val.ToString())
                                    {
                                        currentIndex = i;
                                    }
                                }
                                currentIndex++;
                                if (currentIndex >= enumNames.Length)
                                {
                                    currentIndex = 0;
                                }
                                info.SetValue(controlState.ControlledObject, info.FieldPropertyType.GetEnumValues().GetValue(currentIndex));
                            }
                        }
                    }
                }
            }

            if (midiMessage.Type == MidiMessageType.ControlChange)
            {
                var message = (MidiControlChangeMessage)midiMessage;
                if (message.Controller == 9 || message.Controller == 10)
                {
                    Console.WriteLine("MIDI: Changed " + GetControlName(message.Controller, MidiControlDescriptionType.Knob) + " to " + message.ControlValue);
                    SliderValue = message.ControlValue / 127f;
                }
                else
                {
                    int delta = message.ControlValue - MIDI.KNOB_CENTER;
                    Console.WriteLine("MIDI: Turned " + GetControlName(message.Controller, MidiControlDescriptionType.Knob) + " with a delta of " + delta);

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
                                val += delta * KnobControlStep;
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
                            else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(Vector2))
                            {
                                var val = (Vector2)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                                switch (controlState.VectorIndex)
                                {
                                    case 0:
                                        val.X += delta * KnobControlStep;
                                        break;
                                    case 1:
                                        val.Y += delta * KnobControlStep;
                                        break;
                                    default:
                                        throw new Exception("Trying to change vector index that is out of bounds for this size of vector: " + controlState.ControlledObject);
                                }
                                controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                            }
                            else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(Vector3))
                            {
                                var val = (Vector3)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                                switch (controlState.VectorIndex)
                                {
                                    case 0:
                                        val.X += delta * KnobControlStep;
                                        break;
                                    case 1:
                                        val.Y += delta * KnobControlStep;
                                        break;
                                    case 2:
                                        val.Z += delta * KnobControlStep;
                                        break;
                                    default:
                                        throw new Exception("Trying to change vector index that is out of bounds for this size of vector: " + controlState.ControlledObject);
                                }
                                controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                            }
                            else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(Vector4))
                            {
                                var val = (Vector4)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                                switch (controlState.VectorIndex)
                                {
                                    case 0:
                                        val.X += delta * KnobControlStep;
                                        break;
                                    case 1:
                                        val.Y += delta * KnobControlStep;
                                        break;
                                    case 2:
                                        val.Z += delta * KnobControlStep;
                                        break;
                                    case 3:
                                        val.W += delta * KnobControlStep;
                                        break;
                                    default:
                                        throw new Exception("Trying to change vector index that is out of bounds for this size of vector: " + controlState.ControlledObject);
                                }
                                controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                            }
                            else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(Xna.Vector2))
                            {
                                var val = (Xna.Vector2)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                                switch (controlState.VectorIndex)
                                {
                                    case 0:
                                        val.X += delta * KnobControlStep;
                                        break;
                                    case 1:
                                        val.Y += delta * KnobControlStep;
                                        break;
                                    default:
                                        throw new Exception("Trying to change vector index that is out of bounds for this size of vector: " + controlState.ControlledObject);
                                }
                                controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                            }
                            else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(Xna.Vector3))
                            {
                                var val = (Xna.Vector3)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                                switch (controlState.VectorIndex)
                                {
                                    case 0:
                                        val.X += delta * KnobControlStep;
                                        break;
                                    case 1:
                                        val.Y += delta * KnobControlStep;
                                        break;
                                    case 2:
                                        val.Z += delta * KnobControlStep;
                                        break;
                                    default:
                                        throw new Exception("Trying to change vector index that is out of bounds for this size of vector: " + controlState.ControlledObject);
                                }
                                controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                            }
                            else if (controlState.FieldPropertyInfo.FieldPropertyType == typeof(Xna.Vector4))
                            {
                                var val = (Xna.Vector4)controlState.FieldPropertyInfo.GetValue(controlState.ControlledObject);
                                switch (controlState.VectorIndex)
                                {
                                    case 0:
                                        val.X += delta * KnobControlStep;
                                        break;
                                    case 1:
                                        val.Y += delta * KnobControlStep;
                                        break;
                                    case 2:
                                        val.Z += delta * KnobControlStep;
                                        break;
                                    case 3:
                                        val.W += delta * KnobControlStep;
                                        break;
                                    default:
                                        throw new Exception("Trying to change vector index that is out of bounds for this size of vector: " + controlState.ControlledObject);
                                }
                                controlState.FieldPropertyInfo.SetValue(controlState.ControlledObject, val);
                            }
                        }
                    }
                }
            }
        }

        public void AssignControl(object controlledObject, FieldPropertyInfo fieldPropertyInfo)
        {
            AssignControl(controlledObject, fieldPropertyInfo, lastAssignmentButton);
        }

        public void AssignControl(object controlledObject, string fieldPropertyName, byte assignmentButton)
        {
            var propertyInfo = controlledObject.GetType().GetProperty(fieldPropertyName);
            if (propertyInfo != null)
            {
                AssignControl(controlledObject, new FieldPropertyInfo(propertyInfo), assignmentButton);
            }
            else
            {
                var fieldInfo = controlledObject.GetType().GetField(fieldPropertyName);
                if (fieldInfo != null)
                {
                    AssignControl(controlledObject, new FieldPropertyInfo(fieldInfo), assignmentButton);
                }
                else
                {
                    throw new Exception("Failed to assign control of property or field with name " + fieldPropertyName);
                }
            }
        }

        public void AssignControl(object controlledObject, FieldPropertyInfo fieldPropertyInfo, byte assignmentButton)
        {
            if (ControlStates.ContainsKey(assignmentButton))
            {
                var controlState = ControlStates[assignmentButton];
                controlState.ControlledObject = controlledObject;
                controlState.FieldPropertyInfo = fieldPropertyInfo;

                // See if it's a vector and assign subsequent controls if it is.
                byte vectorSize = 1;
                if (fieldPropertyInfo.FieldPropertyType == typeof(Vector2)
                    || fieldPropertyInfo.FieldPropertyType == typeof(Xna.Vector2))
                {
                    vectorSize = 2;
                }
                else if (fieldPropertyInfo.FieldPropertyType == typeof(Vector3)
                    || fieldPropertyInfo.FieldPropertyType == typeof(Xna.Vector3))
                {
                    vectorSize = 3;
                }
                else if (fieldPropertyInfo.FieldPropertyType == typeof(Vector4)
                    || fieldPropertyInfo.FieldPropertyType == typeof(Xna.Vector4))
                {
                    vectorSize = 4;
                }

                if (vectorSize > 1)
                {
                    controlState.IsVector = true;
                    controlState.VectorIndex = 0;
                    for (byte i = 1; i < vectorSize; i++)
                    {
                        byte vectorAssignmentButton = (byte)(assignmentButton + i);
                        if (AssignToControlMapping.ContainsKey(vectorAssignmentButton)
                            && AssignToControlMapping[vectorAssignmentButton].Type == AssignToControlMapping[assignmentButton].Type) // So long as this control is still the same control type
                        {
                            var vectorControlState = ControlStates[vectorAssignmentButton];
                            vectorControlState.ControlledObject = controlledObject;
                            vectorControlState.FieldPropertyInfo = fieldPropertyInfo;
                            vectorControlState.IsVector = true;
                            vectorControlState.VectorIndex = i;
                            ControlStates[vectorAssignmentButton] = vectorControlState;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                ControlStates[assignmentButton] = controlState;
            }
            Assigning = false;
        }
    }
}
