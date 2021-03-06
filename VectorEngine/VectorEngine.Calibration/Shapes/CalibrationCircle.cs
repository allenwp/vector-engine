﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Calibration.Shapes
{
    /// <summary>
    /// Shows a customizable circle that can reveal channel delays, blanking issues, etc.
    /// </summary>
    public class CalibrationCircle : Shape
    {
        public int SamplesPerCircle = 16;
        [EditorHelper.Help("Set this to be very high to blank for long enough that you don't get framerate issues.")]
        public int BlankingCircles = 200;
        public float ExpansionPerCircle = 0.001f;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);
            int numCircles = BlankingCircles + 1 + BlankingCircles;

            Sample3D[] array = new Sample3D[numCircles * SamplesPerCircle];
            for (int i = 0; i < array.Length; i++)
            {
                double radians = i * (MathHelper.TwoPi / SamplesPerCircle);
                float expansion = 1f + (i - BlankingCircles * SamplesPerCircle) * (ExpansionPerCircle / SamplesPerCircle);
                array[i].Position = new Vector3((float)Math.Sin(radians) * expansion, (float)Math.Cos(radians) * expansion, 0);

                if (i >= BlankingCircles * SamplesPerCircle && i <= (BlankingCircles + 1) * SamplesPerCircle)
                {
                    array[i].Brightness = 1f;
                }
                else
                {
                    array[i].Brightness = 0f;
                }

            }

            result.Add(array);
            return result;
        }
    }
}
