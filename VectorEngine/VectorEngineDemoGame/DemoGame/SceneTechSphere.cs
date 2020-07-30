using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine;
using VectorEngine.Extras;
using VectorEngine.DemoGame.DemoGame.Shapes;

namespace VectorEngine.DemoGame
{
    public class SceneTechSphere
    {
        public static void Init()
        {
            // Order maters here. It's the execution order.
            // "Update" systems:
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new CurlyCircleSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            //EntityAdmin.Instance.CreateCommonSingletons();

            // Create scene objects
            // Order *kinda* matters here: it's the draw order for Shapes

            var cameraEntity = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Transform>(cameraEntity).LocalPosition = new Vector3(0, 0, 3f);
            var camera = EntityAdmin.Instance.AddComponent<Camera>(cameraEntity);
            camera.ProjectionType = Camera.ProjectionTypeEnum.Perspective;
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(cameraEntity);

            var shape = EntityAdmin.Instance.CreateEntity("shape");
            EntityAdmin.Instance.AddComponent<Transform>(shape).LocalPosition = new Vector3(0, 0, 0);
            EntityAdmin.Instance.AddComponent<DemoShape>(shape);
        }
    }
}
