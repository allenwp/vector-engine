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

        static Random random = new Random();

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

            for (int i = 0; i < 4; i++)
            {
                CreateTransforms(null);
            }
        }
        public static List<Transform> CreateTransforms(Transform parent)
        {
            int count = 1;
            var result = new List<Transform>(count);
            for (int i = 0; i < count; i++)
            {
                var entity = EntityAdmin.Instance.CreateEntity("TransformTest" + i);
                var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
                EntityAdmin.Instance.AddComponent<Cube>(entity);
                Transform.AssignParent(trans, parent);

                trans.LocalPosition = new Vector3(Rand(), Rand(), Rand());
                trans.LocalScale = new Vector3(Rand(), Rand(), Rand());
                trans.LocalRotation = Quaternion.CreateFromYawPitchRoll(Rand(), Rand(), Rand());

                if (trans.Parent == null || trans.Parent.Parent == null || trans.Parent.Parent.Parent == null)
                {
                    foreach (var child in CreateTransforms(trans))
                    {
                        Transform.AssignParent(child, trans);
                    }
                }
                result.Add(trans);
            }
            return result;
        }

        public static float Rand()
        {
            return ((float)random.NextDouble() + 0.3f) * (random.Next(2) > 0 ? -1 : 1);
        }
    }
}
