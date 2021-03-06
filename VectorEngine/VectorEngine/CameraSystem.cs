﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine
{
    public class CameraSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach ((var transform, var camera) in EntityAdmin.Instance.GetTuple<Transform, Camera>())
            {
                var worldPosition = transform.Position;
                var worldTarget = worldPosition + Vector3.Transform(Vector3.Forward, transform.Rotation);
                var up = Vector3.Transform(Vector3.Up, transform.Rotation);
                camera.ViewMatrix = Matrix.CreateLookAt(transform.Position, worldTarget, up);

                if (camera.ProjectionType == Camera.ProjectionTypeEnum.Perspective)
                {
                    camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(camera.FoV, FrameOutput.DisplayProfile.AspectRatio, camera.NearPlane, camera.FarPlane);
                }
                else if (camera.ProjectionType == Camera.ProjectionTypeEnum.Orthographic)
                {
                    camera.ProjectionMatrix = Matrix.CreateOrthographic(camera.OrthographicSize * FrameOutput.DisplayProfile.AspectRatio, camera.OrthographicSize, camera.NearPlane, camera.FarPlane);
                }
                else
                {
                    throw new NotImplementedException("Camera type not supported");
                }
            }
        }
    }
}
