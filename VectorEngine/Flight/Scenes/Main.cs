﻿using Flight.PostProcessing;
using Microsoft.Xna.Framework;
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
    public class Main
    {
        public static void Init()
        {
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());

            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new TrackSystem());
            EntityAdmin.Instance.Systems.Add(new FieldSystem());

            // Post Processing Systems:
            EntityAdmin.Instance.Systems.Add(new StrobePostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new PolarCoordinatesPostProcessorSystem());
            EntityAdmin.Instance.Systems.Add(new PolarCoordHorizonMaskPostProcessorSystem());

            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            EntityAdmin.Instance.CreateSingletons();

            var entity = EntityAdmin.Instance.CreateEntity("Camera");
            var cam = EntityAdmin.Instance.AddComponent<Camera>(entity);
            cam.FarPlane = 1000f;
            cam.NearPlane = 1f;
            var camTrans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            camTrans.LocalPosition = new Vector3(0, 0, 100);
            var ppg = EntityAdmin.Instance.AddComponent<PostProcessingGroup3D>(entity);

            entity = EntityAdmin.Instance.CreateEntity("Track Point");
            var trackTrans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            var track = EntityAdmin.Instance.AddComponent<Track>(entity);
            track.TrackPoint = trackTrans;

            var originEntity = EntityAdmin.Instance.CreateEntity("Polar Origin");
            var originTrans = EntityAdmin.Instance.AddComponent<Transform>(originEntity);
            originTrans.LocalPosition.Y = -500;

            var polarPP = EntityAdmin.Instance.AddComponent<PolarCoordinatesPostProcessor>(cam.Entity);
            polarPP.Origin = originTrans;
            polarPP.ZScale = 0.002f;
            ppg.PostProcessors.Add(polarPP);
            var polarMaskPP = EntityAdmin.Instance.AddComponent<PolarCoordHorizonMaskPostProcessor>(cam.Entity);
            polarMaskPP.PolarCoordinates = polarPP;
            polarMaskPP.YCutoff = -5;
            ppg.PostProcessors.Add(polarMaskPP);

            Transform.AssignParent(originTrans, trackTrans);
            Transform.AssignParent(camTrans, trackTrans);

            entity = EntityAdmin.Instance.CreateEntity("Field");
            var field = EntityAdmin.Instance.AddComponent<Field>(entity);
            FieldSystem.PopulateField(field);

        }
    }
}