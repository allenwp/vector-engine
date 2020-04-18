using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class CurlyCircle : Shape
    {
        public int CurlCount = 20;
        public float CurlSize = 0.2f;

        public float AnimationSpeed = 0.1f;
        public float AnimationOffset = 0f;

        int baseSampleCount = 3000;
        public override List<Sample3DStream> GetSamples3D(float fidelity)
        {
            List<Sample3DStream> result = new List<Sample3DStream>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            Sample3DStream sampleStream = Sample3DPool.GetStream(sampleCount);
            for (int i = 0; i < sampleCount; i++)
            {
                var progress = (float)i / (float)(sampleCount - 1);
                progress = Tween.EaseOutPower(progress, 3) + AnimationOffset;

                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), progress);
                var point3D = new Vector3((float)Math.Sin(value), (float)Math.Cos(value), 0);

                var secondValue = MathHelper.Lerp(0, (float)(Math.PI * 2), progress * CurlCount);
                var secondCirclePoint = CurlSize * new Vector3((float)Math.Sin(secondValue), (float)Math.Cos(secondValue), 0);
                point3D += secondCirclePoint;

                sampleStream[i] = new Sample3D() { Position = point3D, Brightness = 1f };
            }
            result.Add(sampleStream);
            return result;
        }
    }
}
