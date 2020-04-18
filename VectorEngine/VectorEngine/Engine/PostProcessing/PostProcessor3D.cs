using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine.PostProcessing
{
    public abstract class PostProcessor3D : Component
    {
        public delegate void PostProcess3DDelegate(List<Sample3DStream> samples3D, PostProcessor3D postProcessor);

        public abstract PostProcess3DDelegate PostProcess3DFuntion { get; }
    }
}
