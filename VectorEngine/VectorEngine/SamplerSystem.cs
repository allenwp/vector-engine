using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine
{
    public class SamplerSystem : ECSSystem
    {
        public override void Tick()
        {
            var cameraTuples = EntityAdmin.Instance.GetTuple<Transform, Camera>();
            var shapeTuples = EntityAdmin.Instance.GetTuple<Transform, Shape>();

            List<Sample[]> result = new List<Sample[]>();

            foreach ((var cameraTransform, var camera) in cameraTuples)
            {
                List<Sample3D[]> worldSpaceResult = new List<Sample3D[]>();

                int highestLayer = 0;
                foreach ((var transform, var shape) in shapeTuples)
                {
                    if (shape.Layer > highestLayer)
                    {
                        highestLayer = shape.Layer;
                    }
                }

                #region Old single threaded code
                //foreach ((var transform, var shape) in shapeTuples.Where(tuple =>
                //    tuple.Item2.Layer == highestLayer
                //    && (camera.Filter & tuple.Item2.CameraFilterLayers) != 0
                //    && !FrustumCull(camera, cameraTransform, tuple.Item1, tuple.Item2)))
                //{
                //    var samples3D = GetSample3Ds(camera, cameraTransform, transform, shape);
                //    worldSpaceResult.AddRange(samples3D);
                //}
                #endregion

                #region Using a Partitioner: Initial tests show that this is marginally slower than simply not having a Partitioner.
                //var tuplesCollection = shapeTuples.Where(tuple =>
                //    tuple.Item2.Layer == highestLayer
                //    && (camera.Filter & tuple.Item2.CameraFilterLayers) != 0
                //    && !FrustumCull(camera, cameraTransform, tuple.Item1, tuple.Item2));

                //(Transform, Shape)[] tuples = tuplesCollection.ToArray();
                //List<Sample3D[]>[] parallelOutputSamples = new List<Sample3D[]>[tuples.Length];

                //var rangePartitioner = Partitioner.Create(0, tuples.Length);

                //Parallel.ForEach(rangePartitioner,
                //    (range, loopState) =>
                //    {
                //        for (int i = range.Item1; i < range.Item2; i++)
                //        {
                //            parallelOutputSamples[i] = GetSample3Ds(camera, cameraTransform, tuples[i].Item1, tuples[i].Item2);
                //        }
                //    });

                //for (int i = 0; i < parallelOutputSamples.Length; i++)
                //{
                //    worldSpaceResult.AddRange(parallelOutputSamples[i]);
                //}
                #endregion

                // Create the world space result using the Parallel library:
                var tuplesCollection = shapeTuples.Where(tuple =>
                    tuple.Item2.Layer == highestLayer
                    && (camera.Filter & tuple.Item2.CameraFilterLayers) != 0
                    && !FrustumCull(camera, cameraTransform, tuple.Item1, tuple.Item2));

                (Transform, Shape)[] tuples = tuplesCollection.ToArray();
                List<Sample3D[]>[] parallelOutputSamples = new List<Sample3D[]>[tuples.Length];

                Parallel.For(0, tuples.Length,
                    i => parallelOutputSamples[i] = GetSample3Ds(camera, cameraTransform, tuples[i].Item1, tuples[i].Item2));

                for (int i = 0; i < parallelOutputSamples.Length; i++)
                {
                    worldSpaceResult.AddRange(parallelOutputSamples[i]);
                }

                // We have now collected all our world space samples. Post process them:
                var camPostProcessor3D = camera.Entity.GetComponent<PostProcessing.PostProcessingGroup3D>();
                if (camPostProcessor3D != null)
                {
                    foreach (var postProcessor in camPostProcessor3D.PostProcessors.Where(comp => comp.IsActive))
                    {
                        postProcessor.PostProcess3DFuntion(worldSpaceResult, postProcessor);
                    }
                }

                // World space samples are now ready to be translated to the screen!
                var screenSpaceResult = TransformSamples3DToScreen(camera, worldSpaceResult);

                // We now have this camera's screen space samples. Post process them:
                var camPostProcessor2D = camera.Entity.GetComponent<PostProcessing.PostProcessingGroup2D>();
                if (camPostProcessor2D != null)
                {
                    foreach (var postProcessor in camPostProcessor2D.PostProcessors.Where(comp => comp.IsActive))
                    {
                        postProcessor.PostProcess2DFuntion(screenSpaceResult, postProcessor);
                    }
                }

                result.AddRange(screenSpaceResult);
            }

            // We now have all camera's screen space samples. Post process them:
            var samplerPostProcessor2D = EntityAdmin.Instance.SingletonSampler.Entity.GetComponent<PostProcessing.PostProcessingGroup2D>();
            if (samplerPostProcessor2D != null)
            {
                foreach (var postProcessor in samplerPostProcessor2D.PostProcessors.Where(comp => comp.IsActive))
                {
                    postProcessor.PostProcess2DFuntion(result, postProcessor);
                }
            }

            EntityAdmin.Instance.SingletonSampler.LastSamples = result;
        }

        public static List<Sample3D[]> GetSample3Ds(in Camera camera, in Transform cameraTransform, in Transform transform, in Shape shape)
        {
            float fidelity;
            if (camera.ProjectionType == Camera.ProjectionTypeEnum.Perspective)
            {
                float distanceFromCamera = Math.Abs(Vector3.Distance(transform.Position, cameraTransform.Position));
                float minDistanceFromCamera = camera.NearPlane;
                if (distanceFromCamera < minDistanceFromCamera)
                {
                    distanceFromCamera = minDistanceFromCamera;
                }

                // Fidelity is relative to the "non-3D" plane equivalent being the distance where half of the camera's FoV shows 1 unit on an axis,
                // which matches the coordinate space that this engine uses, which is -1 to 1 (or 1 unit for half the screen)
                // For a fidelity of 1, the distance from camera must equal (1 / (tan(FoV / 2))) ("TOA" triginometry formula)
                // The first 1 in the following equation is based on half of the camera's vision being 1 unit of screen space ("TOA" triginometry formula)
                // This formula uses the half of the camera's vision being 1 unit to match up with drawing the shape as non-3D
                fidelity = 1f / (distanceFromCamera * (float)Math.Tan(camera.FoV / 2f));
            }
            else if (camera.ProjectionType == Camera.ProjectionTypeEnum.Orthographic)
            {
                fidelity = 1f;
            }
            else
            {
                throw new Exception("Unsupported camera type");
            }

            fidelity *= MathHelper.Max(MathHelper.Max(Math.Abs(transform.Scale.X), Math.Abs(transform.Scale.Y)), Math.Abs(transform.Scale.Z)); // Multiply fidelity by max scale

            // Now we have the fidelity for this shape. Get the samples:
            var samples3D = shape.GetSamples3D(fidelity);

            // Post process the local space samples:
            var shapePostProcessor3D = shape.Entity.GetComponent<PostProcessing.PostProcessingGroup3D>();
            if (shapePostProcessor3D != null)
            {
                foreach (var postProcessor in shapePostProcessor3D.PostProcessors.Where(comp => comp.IsActive))
                {
                    postProcessor.PostProcess3DFuntion(samples3D, postProcessor);
                }
            }

            // Transform the samples into world space and record them in the sample stream:
            TransformSamples3DToWorldSpace(samples3D, transform.WorldTransform);
            return samples3D;
        }

        /// <returns>true if the shape should be culled.</returns>
        public static bool FrustumCull(Camera camera, Transform cameraTransform, Transform transform, Shape shape)
        {
            // TODO: write a frustum culling method
            return false;
        }

        public static void TransformSamples3DToWorldSpace(List<Sample3D[]> samples3D, Matrix worldTransform)
        {
            foreach (var samples3DArray in samples3D)
            {
                for (int i = 0; i < samples3DArray.Length; i++)
                {
                    samples3DArray[i].Position = Vector3.Transform(samples3DArray[i].Position, worldTransform);
                }
            }
        }

        public static List<Sample[]> TransformSamples3DToScreen(Camera camera, List<Sample3D[]> samples3D)
        {
            List<Sample[]> result = new List<Sample[]>();
            foreach (var samples3DArray in samples3D)
            {
                int sampleLength = samples3DArray.Length;

                Sample[] tempSampleArray = null;
                int currentArrayIndex = 0;
                for (int i = 0; i < sampleLength; i++)
                {
                    var worldPos = samples3DArray[i].Position;
                    Vector4 v4 = new Vector4(worldPos, 1);
                    // When samples are disabled, it's the same as when they're clipped
                    bool clipped = samples3DArray[i].Disabled;
                    if (!clipped)
                    {
                        v4 = PerformViewTransform(v4, camera.ViewMatrix);
                        v4 = PerformProjectionTransform(v4, camera.ProjectionMatrix);
                        clipped = Clip(v4);
                    }

                    if (!clipped)
                    {
                        if (tempSampleArray == null)
                        {
                            tempSampleArray = new Sample[sampleLength];
                        }

                        Vector2 result2D = PerformViewportTransform(v4, true, FrameOutput.DisplayProfile.AspectRatio);
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
        public static Vector2 PerformViewportTransform(Vector4 vertex, bool scaleToAspectRatio, float aspectRatio)
        {
            Vector2 viewportSpace = new Vector2(vertex.X / vertex.W, vertex.Y / vertex.W);

            if (scaleToAspectRatio)
            {
                viewportSpace.X *= aspectRatio;
            }

            return viewportSpace;
        }
    }
}
