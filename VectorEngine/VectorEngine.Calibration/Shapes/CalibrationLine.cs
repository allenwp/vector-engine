using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Calibration.Shapes
{
    public class CalibrationLine : Shape
    {
        [EditorHelper.Help("Number of blanking samples before and after.")]
        public int BlankingPadding = 50;
        public List<float> BrightnessPattern = new List<float>() { 1f };
        public int RepeatCount = 1;
        public float StepDistance = 0.001f;
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>();
            Sample3D[] array = new Sample3D[BlankingPadding * 2 + BrightnessPattern.Count * RepeatCount];
            int innerPaddingI = 0;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Position = new Vector3(StepDistance * i - StepDistance * BlankingPadding, 0, 0);
                if (i < BlankingPadding || i >= array.Length- BlankingPadding)
                {
                    array[i].Brightness = 0;
                }
                else
                {
                    array[i].Brightness = BrightnessPattern[innerPaddingI];
                    innerPaddingI++;
                    if (innerPaddingI >= BrightnessPattern.Count)
                    {
                        innerPaddingI = 0;
                    }
                }
            }
            result.Add(array);
            return result;
        }
    }
}
