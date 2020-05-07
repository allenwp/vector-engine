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

            var player = Entity.Create("Player");
            Entity.AddComponent<Transform>(player).LocalScale = new Vector3(0.2f);
            Entity.AddComponent<GamepadBasicFPSMovement>(player);
            //player.AddComponent<PlayerShip>();
            //player.AddComponent<Propulsion>();

            var camera = Entity.Create("Camera");
            Entity.AddComponent<Transform>(camera).LocalPosition = new Vector3(0,0,3f);
            Entity.AddComponent<Camera>(camera);
            var ppGroup = Entity.AddComponent<VectorEngine.PostProcessing.PostProcessingGroup3D>(camera);
            ppGroup.PostProcessors.Add(Entity.AddComponent<PostProcessing.RadialPulsePostProcessor>(camera));
            var follow = Entity.AddComponent<Follow>(camera);
            follow.EntityToFollow = player;
            follow.FollowDistance = 4f;

            //CreateRings();

            var seaEntity = Entity.Create("Sea");
            var sea = Entity.AddComponent<SeaOfWaves>(seaEntity);
            sea.Waves = SeaOfWavesSystem.CreateSea();
        }

        public static void CreateRings()
        {
            for (int i = 0; i < 20; i++)
            {
                var entity = Entity.Create("Ring");
                Entity.AddComponent<Transform>(entity).LocalPosition = new Vector3(0, 0, i * -20f);
                Entity.AddComponent<CurlyCircle>(entity);
            }
        }
    }
}
