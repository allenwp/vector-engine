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
        public object ControlledObject;
        public FieldPropertyInfo FieldPropertyInfo;
        public bool IsVector;
        public byte VectorIndex;
    }
}
