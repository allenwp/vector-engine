using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class Line : Shape
    {
        public static int LineLength = 170;

        public Vector3 Start;
        public Vector3 End;

        public override List<Sample[]> GetSamples(Matrix worldTransform, float stepScale)
        {
            List<Sample[]> result = new List<Sample[]>();
            int sampleLength = (int)Math.Round(LineLength * stepScale);

            Sample[] tempSampleArray = null;
            int currentArrayIndex = 0;
            for(int i = 0; i < sampleLength; i++)
            {
                var point3D = Vector3.Lerp(Start, End, (float)i / (float)sampleLength);

                Vector4 point4D = new Vector4(point3D, 1);
                Vector4 v4 = Vector4.Transform(point4D, worldTransform);
                v4 = Transformer.performViewTransform(v4, Camera.ViewMatrix());
                v4 = Transformer.performProjectionTransform(v4, Camera.ProjectionMatrix());
                bool clipped = Transformer.clip(v4);
                if(!clipped)
                {
                    if(tempSampleArray == null)
                    {
                        tempSampleArray = new Sample[sampleLength];
                    }

                    Vector2 result2D = Transformer.performViewportTransform(v4);
                    tempSampleArray[currentArrayIndex].X = result2D.X;
                    tempSampleArray[currentArrayIndex].Y = result2D.Y;
                    tempSampleArray[currentArrayIndex].Brightness = 1;

                    currentArrayIndex++;
                }
                else
                {
                    // Move on to the next array of samples. This allows blanking to be applied in the case where
                    // a continuous path exits the screen on one side and comes in on the other side of the screen.
                    if(tempSampleArray != null && currentArrayIndex > 0)
                    {
                        var newArray = new Sample[currentArrayIndex];
                        Array.Copy(tempSampleArray, 0, newArray, 0, currentArrayIndex);
                        result.Add(newArray);
                    }
                    tempSampleArray = null;
                    currentArrayIndex = 0;
                }
            }

            if (tempSampleArray != null && currentArrayIndex > 0)
            {
                var newArray = new Sample[currentArrayIndex];
                Array.Copy(tempSampleArray, 0, newArray, 0, currentArrayIndex);
                result.Add(newArray);
            }

            return result;
        }
    }
}
