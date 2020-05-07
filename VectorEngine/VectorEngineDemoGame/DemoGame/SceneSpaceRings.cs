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
    public class SceneSpaceRings
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

            var player = EntityAdmin.Instance.CreateEntity("Player");
            EntityAdmin.Instance.AddComponent<Transform>(player).LocalScale = new Vector3(0.2f);
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(player);
            //player.AddComponent<PlayerShip>();
            //player.AddComponent<Propulsion>();

            var camera = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Transform>(camera).LocalPosition = new Vector3(0,0,3f);
            EntityAdmin.Instance.AddComponent<Camera>(camera);
            var ppGroup = EntityAdmin.Instance.AddComponent<VectorEngine.PostProcessing.PostProcessingGroup3D>(camera);
            ppGroup.PostProcessors.Add(EntityAdmin.Instance.AddComponent<PostProcessing.RadialPulsePostProcessor>(camera));
            var follow = EntityAdmin.Instance.AddComponent<Follow>(camera);
            follow.EntityToFollow = player;
            follow.FollowDistance = 4f;

            //CreateRings();

            var seaEntity = EntityAdmin.Instance.CreateEntity("Sea");
            var sea = EntityAdmin.Instance.AddComponent<SeaOfWaves>(seaEntity);
            sea.Waves = SeaOfWavesSystem.CreateSea();
        }

        public static void CreateRings()
        {
            for (int i = 0; i < 20; i++)
            {
                var entity = EntityAdmin.Instance.CreateEntity("Ring");
                EntityAdmin.Instance.AddComponent<Transform>(entity).LocalPosition = new Vector3(0, 0, i * -20f);
                EntityAdmin.Instance.AddComponent<CurlyCircle>(entity);
            }
        }
    }
}
