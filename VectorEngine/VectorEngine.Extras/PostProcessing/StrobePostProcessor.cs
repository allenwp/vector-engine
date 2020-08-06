using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace VectorEngine.Extras.PostProcessing
{
    [RequiresSystem(typeof(StrobePostProcessorSystem))]
    public class StrobePostProcessor : PostProcessor3D
    {
        [Newtonsoft.Json.JsonIgnore]
        public override PostProcess3DDelegate PostProcess3DFuntion { get => StrobePostProcessorSystem.PostProcess; }

        public float AnimationValue { get; set; } = 0;
        public float AnimationSpeed { get; set; } = 1f;
        public float Scale { get; set; } = 25f;
    }
}
