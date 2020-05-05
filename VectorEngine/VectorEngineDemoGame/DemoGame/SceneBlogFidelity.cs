using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    public class SceneBlogFidelity
    {
        public static void Init()
        {
            // Order maters here. It's the execution order.
            // "Update" systems:
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new CurlyCircleSystem());


            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            // Create scene objects
            // Order *kinda* matters here: it's the draw order for Shapes

            var camera = new Entity();
            camera.AddComponent<Transform>().LocalPosition = new Vector3(0,0,2f);
            camera.AddComponent<Camera>();
            camera.AddComponent<GamepadBasicFPSMovement>();

            //var wiggleCircle = new Entity();
            //wiggleCircle.AddComponent<Transform>();
            //var rotate = wiggleCircle.AddComponent<Rotate>();
            //rotate.Axis = Rotate.AxisEnum.z;
            //wiggleCircle.AddComponent<WigglyCircle>();

            //var wiggleCircle2 = new Entity();
            //var transform = wiggleCircle2.AddComponent<Transform>();
            //transform.LocalScale = new Vector3(0.9f);
            //rotate = wiggleCircle2.AddComponent<Rotate>();
            //rotate.Axis = Rotate.AxisEnum.z;
            //rotate.Speed = rotate.Speed * -1f;
            //wiggleCircle2.AddComponent<WigglyCircle>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        CreateGridPoint(new Vector3(i * 0.5f, (k + 1) * -0.5f, j * 0.5f));
                    }
                }
            }
        }

        public static Entity CreateGridPoint(Vector3 pos)
        {
            var entity = new Entity();
            var trans = entity.AddComponent<Transform>();
            entity.AddComponent<GridPoint>();
            trans.LocalPosition = pos;
            return entity;
        }
    }
}
