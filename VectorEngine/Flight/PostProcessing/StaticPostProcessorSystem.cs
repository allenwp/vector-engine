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
    public class StaticPostProcessorSystem : ECSSystem
    {
        public static void PostProcess(List<Sample[]> samples, PostProcessor2D postProcessor)
        {
            var staticPP = postProcessor as StaticPostProcessor;

            var amount = staticPP.Amount;

            if (amount != 0)
            {
                var rangePartitioner = Partitioner.Create(samples);
                Parallel.ForEach(rangePartitioner, samplesArray =>
                {
                    Random rand = new Random();
                    for (int i = 0; i < samplesArray.Length; i++)
                    {
                        samplesArray[i].X += ((float)rand.NextDouble() - 0.5f) * amount;
                        samplesArray[i].Y += ((float)rand.NextDouble() - 0.5f) * amount;
                    }
                });
            }
        }
    }
}
