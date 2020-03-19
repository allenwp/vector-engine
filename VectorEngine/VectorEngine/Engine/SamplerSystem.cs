using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    public class SamplerSystem : ECSSystem
    {
        public override void Tick()
        {
            List<Sample[]> result = new List<Sample[]>();
            foreach ((Transform transform, Shape shape) in EntityAdmin.Instance.GetTuple<Transform, Shape>())
            {
                // TODO: optimize this by using parallels library

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

                var samples3D = shape.GetSamples3D(fidelity);

                result.AddRange(TransformSamples3DToScreen(samples3D, transform.WorldTransform, transform.Is3D));
            }

            EntityAdmin.Instance.SingletonSampler.LastSamples = result;
        }

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
                        v4 = PerformViewTransform(v4, Camera.ViewMatrix());
                        v4 = PerformProjectionTransform(v4, Camera.ProjectionMatrix());
                        clipped = Clip(v4);
                    }
                    if (!clipped)
                    {
                        if (tempSampleArray == null)
                        {
                            tempSampleArray = new Sample[sampleLength];
                        }

                        Vector2 result2D = PerformViewportTransform(v4, FrameOutput.AspectRatio);
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

        /// <summary>
        /// Produces "view" coordinates (aka "eye" or "camera" coordinates)
        /// Applies the view transform of the given camera (transformCamera.worldToCameraMatrix)
        /// to the homogeonous vertex.
        /// </summary>
        public static Vector4 PerformViewTransform(Vector4 vertex, Matrix worldToCameraMatrix)
        {
            Vector4 result = Vector4.Transform(vertex, worldToCameraMatrix);
            return result;
        }

        /// <summary>
        /// Produces "clip" coordinates by applying the projection transform.
        /// This result can be transformed into "Normalized Device Coordinates" by performing a homogeneous devide.
        /// </summary>
        public static Vector4 PerformProjectionTransform(Vector4 vertex, Matrix projectionMatrix)
        {
            Vector4 result = Vector4.Transform(vertex, projectionMatrix);
            return result;
        }

        public static bool Clip(Vector4 vertex)
        {
            bool result = !(-vertex.W <= vertex.X && vertex.X <= vertex.W
                            && -vertex.W <= vertex.Y && vertex.Y <= vertex.W
                            && -vertex.W <= vertex.Z && vertex.Z <= vertex.W);
            return result;
        }

        /// <summary>
        /// Performs a homogeneous divide and discards z to get the final screen space coordinates
        /// </summary>
        public static Vector2 PerformViewportTransform(Vector4 vertex, float aspectRatio)
        {
            Vector2 viewportSpace = new Vector2(vertex.X / vertex.W, vertex.Y / vertex.W);

            viewportSpace.X *= aspectRatio;

            return viewportSpace;
        }
    }
}
