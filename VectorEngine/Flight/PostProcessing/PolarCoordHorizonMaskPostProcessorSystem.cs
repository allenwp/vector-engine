using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace Flight.PostProcessing
{
    public class PolarCoordHorizonMaskPostProcessorSystem : ECSSystem
    {

        public static void PostProcess(List<Sample3D[]> samples3D, PostProcessor3D postProcessor)
        {
            var mask = postProcessor as PolarCoordHorizonMaskPostProcessor;

            var yCuttoff = mask.YCutoff;

            var rangePartitioner = Partitioner.Create(samples3D);
            Parallel.ForEach(rangePartitioner, samples3DArray =>
            {
                for (int i = 0; i < samples3DArray.Length; i++)
                {
                    // TODO: right now this is just based on a line formed by a hardcoded offset from origin Z and 0 on the Y axis. This *should* be
                    // based on the camera's line of sight.

                    // For now, just disable all samples that are below the horizon
                    if(samples3DArray[i].Position.Z < mask.PolarCoordinates.Origin.Position.Z && samples3DArray[i].Position.Y < yCuttoff)
                    {
                        samples3DArray[i].Brightness = 0f;
                    }
                }
            });
        }
    }
}
