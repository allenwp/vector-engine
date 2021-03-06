﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame.Shapes
{
    public class WaveTile : Shape
    {
        public float AnimationValue;
        public float DrawLength = 1f;

        int baseSampleCount = 500;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            Sample3D[] sample3DArray = new Sample3D[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var progress = (float)i / (float)(sampleCount);
                var value = MathHelper.Lerp(0, (float)(Math.PI * 2), progress);
                var point3D = new Vector3(progress, (float)Math.Sin(value), 0);
                sample3DArray[i].Position = point3D;
                sample3DArray[i].Brightness = 1f;
            }

            result.Add(sample3DArray);
            return result;
        }
    }
}
