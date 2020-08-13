using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class ComponentGroup : Component
    {
        public string FilePath;
        public List<object> RootObjects = new List<object>();
    }
}
