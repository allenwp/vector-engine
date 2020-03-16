using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    public abstract class Shape
    {
        Transform transform;

        public List<Sample[]> GetSamples()
        {
            float fidelity = 1f;

            if (transform.Is3D)
            {
                float distanceFromCamera = Math.Abs(Vector3.Distance(transform.Position, Camera.Position));
                // Fidelity is relative to the "non-3D" plane equivalent being the distance where half of the camera's FoV shows 1 unit on an axis,
                // which matches the coordinate space that this engine uses, which is -1 to 1 (or 1 unit for half the screen)
                // For a fidelity of 1, the distance from camera must equal (1 / (tan(FoV / 2))) ("TOA" triginometry formula)
                // The first 1 in the following equation is based on half of the camera's vision being 1 unit of screen space ("TOA" triginometry formula)
                // This formula uses the half of the camera's vision being 1 unit to match up with drawing the shape as non-3D
                fidelity = 1f / (distanceFromCamera * (float)Math.Tan(Camera.FoV / 2f));
                fidelity *= MathHelper.Max(MathHelper.Max(transform.Scale.X, transform.Scale.Y), transform.Scale.Z); // Multiply fidelity by max scale
            }

            var samples3D = GetSamples3D(fidelity);

            return TransformSamples3DToScreen(samples3D, transform.WorldTransform, transform.Is3D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fidelity">Kind of like a "dynamic level of detail".
        /// It is a scale used to reduce or increase number of resulting samples based on
        /// what physicsal size the shape will be when it is rendered to the screen.
        /// This is based on the worldTransform and camera transforms.</param>
        /// <returns></returns>
        public abstract List<Sample3D[]> GetSamples3D(float fidelity);

        public static List<Sample[]> TransformSamples3DToScreen(List<Sample3D[]> samples3D, Matrix worldTransform, bool is3D)
        {
            List<Sample[]> result = new List<Sample[]>();
            foreach (var samples3DArray in samples3D)
            {
                int sampleLength = samples3DArray.Length;

                Sample[] tempSampleArray = null;
                int currentArrayIndex = 0;
                for (int i = 0; i < sampleLength; i++)
                {
                    Vector4 point4D = new Vector4(samples3DArray[i].Position, 1);
                    Vector4 v4 = Vector4.Transform(point4D, worldTransform);
                    bool clipped = false;
                    if (is3D)
                    {
                        v4 = Transformer.performViewTransform(v4, Camera.ViewMatrix());
                        v4 = Transformer.performProjectionTransform(v4, Camera.ProjectionMatrix());
                        clipped = Transformer.clip(v4);
                    }
                    if (!clipped)
                    {
                        if (tempSampleArray == null)
                        {
                            tempSampleArray = new Sample[sampleLength];
                        }

                        Vector2 result2D = Transformer.performViewportTransform(v4, FrameOutput.AspectRatio);
                        tempSampleArray[currentArrayIndex].X = result2D.X;
                        tempSampleArray[currentArrayIndex].Y = result2D.Y;
                        tempSampleArray[currentArrayIndex].Brightness = samples3DArray[i].Brightness;

                        currentArrayIndex++;
                    }
                    else
                    {
                        // Move on to the next array of samples. This allows blanking to be applied in the case where
                        // a continuous path exits the screen on one side and comes in on the other side of the screen.
                        if (tempSampleArray != null && currentArrayIndex > 0)
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
            }

            return result;
        }
    }
}
