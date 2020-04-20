using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class WaveTileAnimated : Shape
    {
        public float AnimationValue;
        public float DrawLength = 1f;

        int baseSampleCount = 150;
        public override List<Sample3DStream> GetSamples3D(float fidelity)
        {
            List<Sample3DStream> result = new List<Sample3DStream>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            Sample3DStream sampleStream = Sample3DPool.GetStream(sampleCount);
            int finalSampleStartIndex = sampleCount;
            int finalSampleCount = 0;
            for (int i = 0; i < sampleCount; i++)
            {
                var progress = (float)i / (float)(sampleCount);
                if(progress > AnimationValue || progress < AnimationValue - DrawLength)
                {
                    continue;
                }
                if (finalSampleStartIndex > i)
                {
                    finalSampleStartIndex = i;
                }
                finalSampleCount++;
                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), progress);
                var point3D = new Vector3(progress, (float)Math.Sin(value), 0);
                sampleStream[i] = new Sample3D() { Position = point3D, Brightness = 1f };
            }

            var finalStream = Sample3DPool.GetStream(finalSampleCount);
            if (finalSampleCount > 0)
            {
                Array.Copy(sampleStream.Pool, sampleStream.PoolIndex(finalSampleStartIndex), finalStream.Pool, finalStream.PoolIndex(0), finalSampleCount);
            }

            result.Add(finalStream);
            return result;
        }
    }
}
