using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine;
using VectorEngine.PostProcessing;

namespace VectorEngine.DemoGame
{
    public class SceneRotatingCubesAndGridPoints
    {
        public static void Init()
        {
            // Order maters here. It's the execution order.
            // "Update" systems:
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new PostProcessing.StrobePostProcessorSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateSingletons();

            // Create scene objects
            // Order *kinda* matters here: it's the draw order for Shapes

            var camera = EntityAdmin.Instance.CreateEntity("Camera");
            var camTransform = EntityAdmin.Instance.AddComponent<Transform>(camera);
            EntityAdmin.Instance.AddComponent<Camera>(camera);
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(camera);
            camTransform.LocalPosition = new Vector3(2f, 0.5f, 8f);

            var cube1 = CreateCube();
            var cube2 = CreateCube();
            cube2.GetComponent<Transform>().LocalPosition.X += 2f;

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

        public static Entity CreateCube()
        {
            var entity = EntityAdmin.Instance.CreateEntity("Cube");
            EntityAdmin.Instance.AddComponent<Transform>(entity);
            EntityAdmin.Instance.AddComponent<Cube>(entity);
            //RandomlyConfigureRotate(entity.AddComponent<Rotate>());
            var pp = EntityAdmin.Instance.AddComponent<PostProcessingGroup3D>(entity);
            pp.PostProcessors.Add(EntityAdmin.Instance.AddComponent<PostProcessing.StrobePostProcessor>(entity));
            return entity;
        }

        public static Entity CreateGridPoint(Vector3 pos)
        {
            var entity = EntityAdmin.Instance.CreateEntity("Grid Point");
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            EntityAdmin.Instance.AddComponent<GridPoint>(entity);
            RandomlyConfigureRotate(EntityAdmin.Instance.AddComponent<Rotate>(entity));
            trans.LocalPosition = pos;
            return entity;
        }

        static Random rand = new Random();
        public static void RandomlyConfigureRotate(Rotate rotate)
        {
            rotate.Speed = MathHelper.Lerp(0.1f, 0.4f, (float)rand.NextDouble());
            rotate.Axis = (Rotate.AxisEnum)rand.Next(3);
        }
    }
}
