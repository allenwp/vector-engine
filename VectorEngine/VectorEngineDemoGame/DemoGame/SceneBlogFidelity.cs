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
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new CurlyCircleSystem());


            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            // Create scene objects
            // Order *kinda* matters here: it's the draw order for Shapes

            var camera = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Transform>(camera).LocalPosition = new Vector3(0,0,2f);
            EntityAdmin.Instance.AddComponent<Camera>(camera);
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(camera);

            //var wiggleCircle = EntityAdmin.Instance.Create("wiggleCircle");
            //wiggleCircle.AddComponent<Transform>();
            //var rotate = wiggleCircle.AddComponent<Rotate>();
            //rotate.Axis = Rotate.AxisEnum.z;
            //wiggleCircle.AddComponent<WigglyCircle>();

            //var wiggleCircle2 = EntityAdmin.Instance.Create("wiggleCircle2");
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
            var entity = EntityAdmin.Instance.CreateEntity("GridPoint");
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            EntityAdmin.Instance.AddComponent<GridPoint>(entity);
            trans.LocalPosition = pos;
            return entity;
        }
    }
}
