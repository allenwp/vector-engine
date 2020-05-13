using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame.Shapes
{
    public class CurlyCircle : Shape
    {
        public int CurlCount { get; set; } = 20;
        public float CurlSize { get; set; } = 0.2f;

        public float AnimationSpeed { get; set; } = 0.1f;
        public float AnimationOffset { get; set; } = 0f;

        int baseSampleCount = 3000;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            Sample3D[] sample3DArray = new Sample3D[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var progress = (float)i / (float)(sampleCount - 1);
                progress = Tween.EaseOutPower(progress, 3) + AnimationOffset;

                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), progress);
                var point3D = new Vector3((float)Math.Sin(value), (float)Math.Cos(value), 0);

                var secondValue = MathHelper.Lerp(0, (float)(Math.PI * 2), progress * CurlCount);
                var secondCirclePoint = CurlSize * new Vector3((float)Math.Sin(secondValue), (float)Math.Cos(secondValue), 0);
                point3D += secondCirclePoint;

                sample3DArray[i].Position = point3D;
                sample3DArray[i].Brightness = 1f;
            }
            result.Add(sample3DArray);
            return result;
        }
    }
}
