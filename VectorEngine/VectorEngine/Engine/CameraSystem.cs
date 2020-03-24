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
            foreach ((Transform transform, Camera camera) in EntityAdmin.Instance.GetTuple<Transform, Camera>())
            {
                var worldPosition = transform.Position;
                var worldTarget = worldPosition + Vector3.Transform(Vector3.Forward, transform.Rotation);
                var up = Vector3.Transform(Vector3.Up, transform.Rotation);
                camera.ViewMatrix = Matrix.CreateLookAt(transform.Position, worldTarget, up);

                camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(camera.FoV, FrameOutput.AspectRatio, camera.NearPlane, camera.FarPlane);
            }
        }
    }
}
