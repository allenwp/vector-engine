using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace VectorEngine.DemoGame.PostProcessing
{
    [RequiresSystem(typeof(StaticPostProcessorSystem))]
    public class StaticPostProcessor : PostProcessor2D
    {
        public float Amount { get; set; } = 0;

        [Newtonsoft.Json.JsonIgnore]
        public override PostProcess2DDelegate PostProcess2DFuntion => StaticPostProcessorSystem.PostProcess;
    }
}
