using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public class Shape
    {
        /// <summary>
        /// If false, this does not exist as a part of the scene graph and doesn't need to be transformed at all.
        /// </summary>
        public bool Is3D;

        // TODO: Figure out scene graph, etc.
        // To sample a path, it needs a transform...
        // public Transform (???)

        public List<Path> Paths;
    }
}
