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
    public class SceneEditorTest
    {
        public static void Init()
        {
            // Order maters here. It's the execution order.
            // "Update" systems:
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new PropulsionSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new FollowSystem());
            EntityAdmin.Instance.Systems.Add(new SeaOfWavesSystem());
            EntityAdmin.Instance.Systems.Add(new CurlyCircleSystem());
            
            // Post Processing Systems
            EntityAdmin.Instance.Systems.Add(new PostProcessing.RadialPulsePostProcessorSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            // Create scene objects
            // Order *kinda* matters here: it's the draw order for Shapes

            var player = new Entity("Player");
            var trans = player.AddComponent<Transform>();
            trans.LocalScale = new Vector3(0.2f);
            player.AddComponent<GamepadBasicFPSMovement>();
            //player.AddComponent<PlayerShip>();
            //player.AddComponent<Propulsion>();

            var camera = new Entity("Camera");
            trans = camera.AddComponent<Transform>();
            trans.LocalPosition = new Vector3(0,0,3f);
            camera.AddComponent<Camera>();
            var ppGroup = camera.AddComponent<VectorEngine.PostProcessing.PostProcessingGroup3D>();
            ppGroup.PostProcessors.Add(camera.AddComponent<PostProcessing.RadialPulsePostProcessor>());
            var follow = camera.AddComponent<Follow>();
            follow.EntityToFollow = player;
            follow.FollowDistance = 4f;

            var transforms = CreateTransforms(null);

            var seaEntity = new Entity("Sea");
            var sea = seaEntity.AddComponent<SeaOfWaves>();
            sea.Waves = SeaOfWavesSystem.CreateSea();
        }

        public static List<Transform> CreateTransforms(Transform parent)
        {
            int count = 5;
            var result = new List<Transform>(count);
            for (int i = 0; i < count; i++)
            {
                var entity = new Entity("TransformTest" + i);
                if(i == 1)
                {
                    entity.Enabled = false;
                }
                var trans = entity.AddComponent<Transform>();
                if (i == 2)
                {
                    trans.Enabled = false;
                }
                Transform.AssignParent(trans, parent);
                trans.LocalPosition = new Vector3(0, 0, i * -20f);
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
    }
}
