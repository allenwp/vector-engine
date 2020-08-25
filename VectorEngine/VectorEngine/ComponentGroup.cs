using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class ComponentGroup : Component
    {
        public static readonly string FILE_EXTENSION = "cpg";
        public static readonly string ROOT_PATH = "Component Groups";
        public string ComponentGroupPath
        {
            get => Path.Combine(ROOT_PATH, $"{FileName}.{FILE_EXTENSION}");
        }

        public string FileName;
        public List<object> RootObjects = new List<object>();
    }
}
