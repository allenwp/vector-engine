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
    public class HyperbolicCoordinatesPostProcessorSystem : ECSSystem
    {
        public override void Tick()
        {

        }

        public static void PostProcess(List<Sample3D[]> samples3D, PostProcessor3D postProcessor)
        {
            HyperbolicCoordinatesPostProcessor hyperbolicPP = postProcessor as HyperbolicCoordinatesPostProcessor;

            var origin = hyperbolicPP.Origin.Position;
            var zScale = hyperbolicPP.ZScale;

            var rangePartitioner = Partitioner.Create(samples3D);
            Parallel.ForEach(rangePartitioner, (samples3DArray) =>
            {
                for (int i = 0; i < samples3DArray.Length; i++)
                {
                    var newPosition = samples3DArray[i].Position - origin;
                    newPosition.Z *= zScale;

                    // https://stackoverflow.com/questions/23744120/conversion-from-rectangular-to-hyperbolic-coordinates-not-reversible
                    newPosition = new Vector3(newPosition.X, (float)Math.Sqrt(newPosition.Z * newPosition.Y), (float)Math.Log(newPosition.Y / newPosition.Z));

                    newPosition += origin;
                    samples3DArray[i].Position = newPosition;
                }
            });
        }
    }
}
