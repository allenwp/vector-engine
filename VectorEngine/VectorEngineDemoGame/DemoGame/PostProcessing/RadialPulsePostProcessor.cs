using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.PostProcessing;

namespace VectorEngine.DemoGame.PostProcessing
{
    public class RadialPulsePostProcessor : PostProcessor3D
    {
        public override PostProcess3DDelegate PostProcess3DFuntion { get => RadialPulsePostProcessorSystem.PostProcess; }


        [Category("Runtime State")]
        public float AnimationValue { get; set; } = 0;
        [Category("Movement Config")]
        public float AnimationSpeed { get; set; } = 1f;
        [Category("Size Config")]
        [Description("The max distance that the pulse will travel.")]
        public float MaxDistance { get; set; } = 100f;
        [Category("Size Config")]
        [Description("The width of the visible portion of the pulse.")]
        public float Width { get; set; } = 20f;
        
        [Category("Runtime State")]
        public float CurrentMinDistance { get; set; } = 0;
        [Category("Runtime State")]
        public float CurrentMaxDistance { get; set; } = 0;
        [Category("Runtime State")]
        public Vector3 Position { get; set; }
    }
}
