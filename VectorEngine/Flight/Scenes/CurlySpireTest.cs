using Flight.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Extras;
using VectorEngine.Extras.PostProcessing;
using VectorEngine.PostProcessing;

namespace Flight.Scenes
{
    public class CurlySpireTest
    {
        public static void Init()
        {
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new PostProcessing.PolarCoordinatesPostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new StrobePostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new PlayerShipShapesSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateCommonSingletons();

            var entity = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Camera>(entity);
            var camTrans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            camTrans.LocalPosition = new Microsoft.Xna.Framework.Vector3(0, 0.5f, 1.3f);
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(entity);

            //// Player
            //entity = EntityAdmin.Instance.CreateEntity("Player Ship");
            //var shipTransform = EntityAdmin.Instance.AddComponent<Transform>(entity);
            //var shipShapes = EntityAdmin.Instance.AddComponent<PlayerShipShapes>(entity);
            //for (int i = 0; i < 10; i++)
            //{
            //    var shipRingEntity = EntityAdmin.Instance.CreateEntity("Player Ship Shape " + i);
            //    var shipRingTransform = EntityAdmin.Instance.AddComponent<Transform>(shipRingEntity);
            //    EntityAdmin.Instance.AddComponent<PlayerShipRing>(shipRingEntity);

            //    shipShapes.RingShapes.Add(shipRingTransform);
            //    Transform.AssignParent(shipRingTransform, shipTransform);
            //}

            CreateCurlySpire();
        }

        public static void CreateCurlySpire()
        {
            var entity = EntityAdmin.Instance.CreateEntity("CurlySpire");
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            trans.LocalScale.Z = trans.LocalScale.X = 0.4f;
            EntityAdmin.Instance.AddComponent<CurlySpire>(entity);
            //EntityAdmin.Instance.AddComponent<Rotate>(entity); // meh, I could go either way
            var strobe = EntityAdmin.Instance.AddComponent<StrobePostProcessor>(entity);
            strobe.AnimationSpeed = 1.7f;
            strobe.Scale = -33f;
            var ppg = EntityAdmin.Instance.AddComponent<PostProcessingGroup3D>(entity);
            ppg.PostProcessors.Add(strobe);
        }
    }
}
