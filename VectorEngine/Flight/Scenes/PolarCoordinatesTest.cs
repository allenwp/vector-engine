using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Extras;
using VectorEngine.PostProcessing;

namespace Flight.Scenes
{
    public class PolarCoordinatesTest
    {
        public static void Init()
        {
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new PropulsionSystem());
            EntityAdmin.Instance.Systems.Add(new PostProcessing.PolarCoordinatesPostProcessorSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateSingletons();

            var entity = EntityAdmin.Instance.CreateEntity("Camera");
            EntityAdmin.Instance.AddComponent<Camera>(entity);
            var camTrans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            camTrans.LocalPosition.Y = 52f;
            EntityAdmin.Instance.AddComponent<GamepadBasicFPSMovement>(entity);
            var ppg = EntityAdmin.Instance.AddComponent<PostProcessingGroup3D>(entity);

            var originEntity = EntityAdmin.Instance.CreateEntity("Polar Origin");
            var originTrans = EntityAdmin.Instance.AddComponent<Transform>(originEntity);
            originTrans.LocalPosition.Z = 5;
            originTrans.LocalPosition.Y = -50;
            EntityAdmin.Instance.AddComponent<Propulsion>(originEntity);
            var polarPP = EntityAdmin.Instance.AddComponent<PostProcessing.PolarCoordinatesPostProcessor>(entity);
            polarPP.Origin = originTrans;
            ppg.PostProcessors.Add(polarPP);

            Transform.AssignParent(camTrans, originTrans);

            for (int ix = 0; ix < 4; ix++)
            {
                for (int iz = 0; iz < 30; iz++)
                {
                    var point = new Vector3((ix / 4f) * 2f - 1f, 3f, (iz / 30f) * 10f - 5f);
                    AddPoint(point);
                    point.Y = 1f;
                    AddPoint(point);
                    point.Y = 2f;
                    AddPoint(point);
                    point.Y = 3f;
                    AddPoint(point);
                    point.Y = 4f;
                }
            }
        }

        static void AddPoint(Vector3 pos)
        {
            var entity = EntityAdmin.Instance.CreateEntity("Point " + pos.X + ", " + pos.Y + ", " + pos.Z);
            EntityAdmin.Instance.AddComponent<Transform>(entity).LocalPosition = pos;
            EntityAdmin.Instance.AddComponent<Shapes.Dot>(entity);
        }
    }
}
