using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;

namespace VectorEngine.DemoGame
{
    public class SceneSquareCalibration
    {
        public static void Init()
        {
            // Order maters here. It's the execution order.
            // "Update" systems:
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());


            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateSingletons();

            var entity = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Transform>(entity).LocalPosition = new Vector3(0, 0, 2f);
            var cam = EntityAdmin.Instance.AddComponent<Camera>(entity);
            cam.ProjectionType = Camera.ProjectionTypeEnum.Orthographic;

            CreateCube(1.6f);
            CreateCube(1.4f);
            CreateCube(1.2f);
            //CreateCube(1);
            CreateCube(0.8f);
            //CreateCube(0.6f);
            CreateCube(0.4f);
            CreateCube(0.2f);
        }

        static void CreateCube(float scale)
        {
            var entity = EntityAdmin.Instance.CreateEntity("Cube");
            EntityAdmin.Instance.AddComponent<Transform>(entity).LocalScale = new Vector3(scale, scale, scale);
            var cube = EntityAdmin.Instance.AddComponent<Cube>(entity);
            cube.RecreateLines(40);
        }
    }
}
