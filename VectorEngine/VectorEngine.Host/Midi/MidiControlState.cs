using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Host.Reflection;

namespace VectorEngine.Host.Midi
{
    public struct MidiControlState
    {
        // ControlledObject currently leaks. TODO: clear out references to this
        // MidiControlState when no refereneces to ControlledObject exist in the game.
        public object ControlledObject;
        public FieldPropertyInfo FieldPropertyInfo;
        public bool IsVector;
        public byte VectorIndex;
    }
}
