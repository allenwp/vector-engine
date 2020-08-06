using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.PostProcessing
{
    [RequiresSystem(typeof(SamplerSystem))]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public abstract class PostProcessor2D : Component
    {
        public delegate void PostProcess2DDelegate(List<Sample[]> samples, PostProcessor2D postProcessor);

        public abstract PostProcess2DDelegate PostProcess2DFuntion { get; }
    }
}
