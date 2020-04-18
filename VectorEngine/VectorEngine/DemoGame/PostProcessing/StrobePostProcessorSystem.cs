using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;
using VectorEngine.Engine.PostProcessing;

namespace VectorEngine.DemoGame.PostProcessing
{
    public class StrobePostProcessorSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var strobe in EntityAdmin.Instance.GetComponents<StrobePostProcessor>())
            {
                strobe.AnimationValue += GameTime.LastFrameTime * strobe.AnimationSpeed;
                while (strobe.AnimationValue > 1f)
                {
                    strobe.AnimationValue -= 1f;
                }
            }
        }

        public static void PostProcess(List<Sample3DStream> samples3D, PostProcessor3D postProcessor)
        {
            StrobePostProcessor strobe = postProcessor as StrobePostProcessor;
            float animationValue = strobe.AnimationValue * (float)Math.PI * 2f;
            foreach (var stream in samples3D)
            {
                int sampleLength = stream.Length;
                for (int i = 0; i < sampleLength; i++)
                {
                    var poolIndex = stream.PoolIndex(i);
                    var sinValue = Math.Sin((stream.Pool[poolIndex].Position.Y * strobe.Scale) + animationValue);
                    stream.Pool[poolIndex].Disabled = sinValue < 0;
                }
            }
        }
    }
}
