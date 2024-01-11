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

namespace VectorEngine.DemoGame.PostProcessing
{
    public class PolarCoordinatesPostProcessorSystem : ECSSystem
    {
        public override void Tick()
        {

        }

        public static void PostProcess(List<Sample3D[]> samples3D, PostProcessor3D postProcessor)
        {
            PolarCoordinatesPostProcessor polarPP = postProcessor as PolarCoordinatesPostProcessor;

            var origin = polarPP.Origin.Position;
            var zScale = polarPP.ZScale;

            var rangePartitioner = Partitioner.Create(samples3D);
            Parallel.ForEach(rangePartitioner, samples3DArray =>
            {
                for (int i = 0; i < samples3DArray.Length; i++)
                {
                    var newPosition = samples3DArray[i].Position - origin;
                    var scaledZ = newPosition.Z* zScale; // TODO: z-scale based on distance of "ground" from origin??? This might be possilbe to make this automatic based on that...
                    if (scaledZ > Math.PI / 2f || scaledZ < -1 * Math.PI / 2f)
                    {
                        samples3DArray[i].Brightness = 0f;
                    }
                    else
                    {
                        newPosition = Vector3.Transform(new Vector3(newPosition.X, newPosition.Y, 0f), Quaternion.CreateFromYawPitchRoll(0, scaledZ, 0));

                        newPosition += origin;
                        samples3DArray[i].Position = newPosition;
                    }
                }
            });
        }
    }
}
