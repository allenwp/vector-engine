using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace VectorEngine.DemoGame.PostProcessing
{
    public class RadialPulsePostProcessorSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach ((var transform, var pulse) in EntityAdmin.Instance.GetTuple<Transform, RadialPulsePostProcessor>())
            {
                //pulse.Position = transform.Position;

                pulse.AnimationValue += GameTime.LastFrameTime * pulse.AnimationSpeed;
                while (pulse.AnimationValue > 1f)
                {
                    pulse.AnimationValue -= 1f;
                }

                pulse.CurrentMinDistance = MathHelper.Clamp(MathHelper.Lerp(0 - pulse.Width, pulse.MaxDistance, pulse.AnimationValue), 0, pulse.MaxDistance);
                pulse.CurrentMaxDistance = MathHelper.Clamp(MathHelper.Lerp(0, pulse.MaxDistance + pulse.Width, pulse.AnimationValue), 0, pulse.MaxDistance);
            }
        }

        public static void PostProcess(List<Sample3D[]> samples3D, PostProcessor3D postProcessor)
        {
            RadialPulsePostProcessor pulse = postProcessor as RadialPulsePostProcessor;

            var pos = pulse.Position;
            var min = pulse.CurrentMinDistance;
            var max = pulse.CurrentMaxDistance;
            var rangePartitioner = Partitioner.Create(samples3D);
            Parallel.ForEach(rangePartitioner, samples3DArray =>
            {
                for (int i = 0; i < samples3DArray.Length; i++)
                {
                    float distance = Vector3.Distance(samples3DArray[i].Position, pos);
                    if (distance > max || distance < min)
                    {
                        samples3DArray[i].Brightness = 0f;
                    }
                }
            });
        }
    }
}
