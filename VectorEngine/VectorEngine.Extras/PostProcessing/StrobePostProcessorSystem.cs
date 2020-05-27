using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace VectorEngine.Extras.PostProcessing
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

        public static void PostProcess(List<Sample3D[]> samples3D, PostProcessor3D postProcessor)
        {
            StrobePostProcessor strobe = postProcessor as StrobePostProcessor;
            float animationValue = strobe.AnimationValue * (float)Math.PI * 2f;
            foreach (var samples3DArray in samples3D)
            {
                int sampleLength = samples3DArray.Length;
                for (int i = 0; i < sampleLength; i++)
                {
                    var sinValue = Math.Sin((samples3DArray[i].Position.Y * strobe.Scale) + animationValue);
                    samples3DArray[i].Disabled = sinValue < 0;
                }
            }
        }
    }
}
