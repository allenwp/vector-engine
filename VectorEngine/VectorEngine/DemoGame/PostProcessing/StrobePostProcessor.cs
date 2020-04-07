using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.PostProcessing
{
    public class StrobePostProcessor : PostProcessorLocal3D
    {
        public override PostProcessLocal3DDelegate PostProcessLocal3DFuntion { get { return StrobePostProcessorSystem.PostProcess; } }

        public float AnimationValue = 0;
        public float AnimationSpeed = 1f;
        public float Scale = 25f;
    }
}
