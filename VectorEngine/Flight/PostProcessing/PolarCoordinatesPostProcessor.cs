using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace Flight.PostProcessing
{
    [RequiresSystem(typeof(PolarCoordinatesPostProcessorSystem))]
    public class PolarCoordinatesPostProcessor : PostProcessor3D
    {
        public Transform Origin { get; set; }
        public float ZScale { get; set; } = 0.1f;

        public override PostProcess3DDelegate PostProcess3DFuntion => PolarCoordinatesPostProcessorSystem.PostProcess;
    }
}
