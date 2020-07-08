﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Extras;
using VectorEngine.Extras.PostProcessing;
using VectorEngine.PostProcessing;
using VectorEngine.DemoGame.PostProcessing;
using VectorEngine.DemoGame.DemoGame.MIDIDemo;
using VectorEngine.EditorHelper;
using VectorEngine.DemoGame.Shapes;

namespace VectorEngine.DemoGame.MIDIDemo
{
    public class SceneMIDIDemo
    {
        public static void Init()
        {
            EntityAdmin.Instance.Systems.Add(new GameTimeSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());

            EntityAdmin.Instance.Systems.Add(new RotateSystem());

            EntityAdmin.Instance.Systems.Add(new StaticBurstCollisionSystem());
            EntityAdmin.Instance.Systems.Add(new StaticBurstSystem());

            // Post Processing Systems:
            EntityAdmin.Instance.Systems.Add(new StrobePostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new PolarCoordinatesPostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new PolarCoordHorizonMaskPostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new StaticPostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new RadialPulsePostProcessorSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateSingletons();

            // Camera
            var entity = EntityAdmin.Instance.CreateEntity("Camera");
            var cam = EntityAdmin.Instance.AddComponent<Camera>(entity);
            cam.FarPlane = 1000f;
            cam.NearPlane = 1f;
            var camTrans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            camTrans.LocalPosition = new Vector3(0, 0, 174.826f);
            //camTrans.LocalRotation = Quaternion.CreateFromYawPitchRoll(0, -0.2f, 0);
            var ppg = EntityAdmin.Instance.AddComponent<PostProcessingGroup3D>(entity);
            var ppg2D = EntityAdmin.Instance.AddComponent<PostProcessingGroup2D>(entity);

            var originEntity = EntityAdmin.Instance.CreateEntity("Polar Origin");
            var originTrans = EntityAdmin.Instance.AddComponent<Transform>(originEntity);
            originTrans.LocalPosition.Y = -100;

            // Post Processing
            var radialPP = EntityAdmin.Instance.AddComponent<RadialPulsePostProcessor>(cam.Entity);
            radialPP.SelfEnabled = false;
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(4, radialPP, "AnimationSpeed"));
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(5, radialPP, "Width"));
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(20, radialPP, "SelfEnabled"));
            ppg.PostProcessors.Add(radialPP);

            var polarPP = EntityAdmin.Instance.AddComponent<PolarCoordinatesPostProcessor>(cam.Entity);
            polarPP.Origin = originTrans;
            polarPP.ZScale = 0.002f;
            polarPP.SelfEnabled = false;
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(3, polarPP, "ZScale"));
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(19, polarPP, "SelfEnabled"));
            ppg.PostProcessors.Add(polarPP);

            var staticPP = EntityAdmin.Instance.AddComponent<StaticPostProcessor>(cam.Entity);
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(2, staticPP, "Amount"));
            ppg2D.PostProcessors.Add(staticPP);
            EntityAdmin.Instance.AddComponent<StaticBurst>(cam.Entity).SelfEnabled = false;

            // Shapes
            var dotDisk1 = DotsDisk.CreateDisk(50, 100);
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(0, dotDisk1.Entity.GetComponent<Rotate>(), "Speed"));
            var dotDisk2 = DotsDisk.CreateDisk(50, 100);
            StartupMIDIAssignments.Assignments.Add(new StartupMIDIAssignments(1, dotDisk2.Entity.GetComponent<Rotate>(), "Speed"));

            CreateSpire(new Vector3(0, 0, 50f));
            CreateSpire(new Vector3(25f, 0, 25f));
            CreateSpire(new Vector3(-25f, 0, 5f));
        }

        public static void CreateSpire(Vector3 pos)
        {
            var entity = EntityAdmin.Instance.CreateEntity("Spire");
            EntityAdmin.Instance.AddComponent<CurlySpire>(entity);
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            trans.LocalPosition = pos;
            trans.LocalScale = new Vector3(10f);
            EntityAdmin.Instance.AddComponent<Rotate>(entity);
        }
    }
}
