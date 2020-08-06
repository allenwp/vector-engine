using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.PostProcessing
{
    [RequiresSystem(typeof(SamplerSystem))]
    public abstract class PostProcessor2D : Component
    {
        public delegate void PostProcess2DDelegate(List<Sample[]> samples, PostProcessor2D postProcessor);

        [Newtonsoft.Json.JsonIgnore]
        public abstract PostProcess2DDelegate PostProcess2DFuntion { get; }
    }
}
