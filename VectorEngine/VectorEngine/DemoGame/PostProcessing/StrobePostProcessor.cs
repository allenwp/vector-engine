using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;
using VectorEngine.Engine.PostProcessing;

namespace VectorEngine.DemoGame.PostProcessing
{
    public class StrobePostProcessor : PostProcessor3D
    {
        public override PostProcess3DDelegate PostProcess3DFuntion { get { return StrobePostProcessorSystem.PostProcess; } }

        public float AnimationValue = 0;
        public float AnimationSpeed = 1f;
        public float Scale = 25f;
    }
}
