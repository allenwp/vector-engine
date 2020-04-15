using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Engine
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

                if (camera.Type == Camera.TypeEnum.Perspective)
                {
                    camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(camera.FoV, FrameOutput.AspectRatio, camera.NearPlane, camera.FarPlane);
                }
                else if (camera.Type == Camera.TypeEnum.Orthographic)
                {
                    camera.ProjectionMatrix = Matrix.CreateOrthographic(camera.Size * FrameOutput.AspectRatio, camera.Size, camera.NearPlane, camera.FarPlane);
                }
                else
                {
                    throw new NotImplementedException("Camera type not supported");
                }
            }
        }
    }
}
