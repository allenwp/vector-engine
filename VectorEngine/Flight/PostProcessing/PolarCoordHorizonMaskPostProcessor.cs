using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace Flight.PostProcessing
{
    [RequiresSystem(typeof(PolarCoordHorizonMaskPostProcessorSystem))]
    public class PolarCoordHorizonMaskPostProcessor : PostProcessor3D
    {
        public PolarCoordinatesPostProcessor PolarCoordinates;
        
        /// <summary>
        /// TODO: Get rid of this and make it base don camera's line of sight
        /// </summary>
        public float YCutoff { get; set; } = 0f;

        public override PostProcess3DDelegate PostProcess3DFuntion => PolarCoordHorizonMaskPostProcessorSystem.PostProcess;
    }
}
