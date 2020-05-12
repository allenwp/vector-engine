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
            EntityAdmin.Instance.Systems.Add(new PropulsionSystem());
            EntityAdmin.Instance.Systems.Add(new PostProcessing.PolarCoordinatesPostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new StrobePostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateSingletons();

            var entity = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Camera>(entity);
            var camTrans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(entity);

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
