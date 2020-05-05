using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine.PostProcessing;

namespace VectorEngine.DemoGame.PostProcessing
{
    public class RadialPulsePostProcessor : PostProcessor3D
    {
        public override PostProcess3DDelegate PostProcess3DFuntion { get => RadialPulsePostProcessorSystem.PostProcess; }

        public Vector3 Position;

        public float AnimationValue = 0;
        public float AnimationSpeed = 1f;
        public float MaxDistance = 100f;
        public float Width = 20f;

        public float CurrentMinDistance = 0;
        public float CurrentMaxDistance = 0;
    }
}
