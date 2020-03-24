using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class SceneSpaceRings
    {
        public static void Init()
        {
            // Order maters here. It's the execution order.
            // "Update" systems:
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());


            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            // Create scene objects
            // Order *kinda* matters here: it's the draw order for Shapes

            var camera = new Entity();
            var camTransform = camera.AddComponent<Transform>();
            camera.AddComponent<Camera>();
            camera.AddComponent<GamepadBasicFPSMovement>();
            camTransform.LocalPosition = new Vector3(0, 0, 4f);

            var circle = new Entity();
            circle.AddComponent<Transform>();
            var rotate = circle.AddComponent<Rotate>();
            var compositeShape = circle.AddComponent<CompositeShape>();
            compositeShape.BaseShape = Circle.GetCircle;
            compositeShape.SecondShape = Circle.GetCircle;
            compositeShape.SecondSampleLengthScale = 0.05f;
            compositeShape.SecondSampleScale = new Vector3(0.2f);
        }
    }
}
