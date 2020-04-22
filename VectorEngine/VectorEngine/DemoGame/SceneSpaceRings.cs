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

            var player = new Entity();
            player.AddComponent<Transform>().LocalScale = new Vector3(0.2f);
            player.AddComponent<GamepadBasicFPSMovement>();
            //player.AddComponent<PlayerShip>();
            //player.AddComponent<Propulsion>();

            var camera = new Entity();
            camera.AddComponent<Transform>().LocalPosition = new Vector3(0,0,3f);
            camera.AddComponent<Camera>();
            var ppGroup = camera.AddComponent<Engine.PostProcessing.PostProcessingGroup3D>();
            ppGroup.PostProcessors.Add(camera.AddComponent<PostProcessing.RadialPulsePostProcessor>());
            var follow = camera.AddComponent<Follow>();
            follow.EntityToFollow = player;
            follow.FollowDistance = 4f;

            //CreateRings();

            var seaEntity = new Entity();
            var sea = seaEntity.AddComponent<SeaOfWaves>();
            sea.Waves = SeaOfWavesSystem.CreateSea();
        }

        public static void CreateRings()
        {
            for (int i = 0; i < 20; i++)
            {
                var entity = new Entity();
                entity.AddComponent<Transform>().LocalPosition = new Vector3(0, 0, i * -20f);
                entity.AddComponent<CurlyCircle>();
            }
        }
    }
}
