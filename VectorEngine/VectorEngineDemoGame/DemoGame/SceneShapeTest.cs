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
    public class SceneShapeTest
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

            var camera = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Transform>(camera).LocalPosition = new Vector3(0, 0, 3f);
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

            var curlyCircle = EntityAdmin.Instance.CreateEntity("curlyCircle");
            EntityAdmin.Instance.AddComponent<Transform>(curlyCircle).LocalPosition = new Vector3(0, 0, 0); ;
            EntityAdmin.Instance.AddComponent<CurlyCircle>(curlyCircle);
            EntityAdmin.Instance.AddComponent<Rotate>(curlyCircle).Speed = 0.1f;
        }
    }
}
