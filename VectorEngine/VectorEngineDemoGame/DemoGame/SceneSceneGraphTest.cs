using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;

namespace VectorEngine.DemoGame
{
    public class SceneSceneGraphTest
    {
        public static void Init()
        {
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateSingletons();

            var entity = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Camera>(entity);
            EntityAdmin.Instance.AddComponent<Transform>(entity).LocalPosition = new Vector3(0,0,2f);
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(entity);


            entity = EntityAdmin.Instance.CreateEntity("Root GridPoint");
            EntityAdmin.Instance.AddComponent<Rotate>(entity);
            EntityAdmin.Instance.AddComponent<Transform>(entity);
            EntityAdmin.Instance.AddComponent<GridPoint>(entity);

            entity = EntityAdmin.Instance.CreateEntity("Root GridPoint");
            EntityAdmin.Instance.AddComponent<Rotate>(entity);
            EntityAdmin.Instance.AddComponent<Transform>(entity).LocalPosition = new Vector3(0.5f, 0, 0f);
            EntityAdmin.Instance.AddComponent<GridPoint>(entity);
        }
    }
}
