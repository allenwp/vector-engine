using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.EditorHelper
{
    public class StartupMIDIAssignments
    {
        public byte AssignmentButton;
        public object Target;
        public string FieldPropertyName;

        public StartupMIDIAssignments(byte assignmentButton, object target, string fieldPropertyName)
        {
            AssignmentButton = assignmentButton;
            Target = target;
            FieldPropertyName = fieldPropertyName;
        }
    }
}
