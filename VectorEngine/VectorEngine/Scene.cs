using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.EditorHelper;

namespace VectorEngine
{
    public class Scene
    {
        public List<Component> Components = new List<Component>();

        List<StartupMIDIAssignments> StartupMIDIAssignments = new List<StartupMIDIAssignments>();
    }
}
