﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    public class SceneBlankingTest
    {
        public static void Init()
        {
            // Order maters here. It's the execution order.
            // "Update" systems:
            EntityAdmin.Instance.Systems.Add(new GamepadSystem());
            EntityAdmin.Instance.Systems.Add(new RotateSystem());
            EntityAdmin.Instance.Systems.Add(new CurlyCircleSystem());


            // "Draw" systems:
            EntityAdmin.Instance.Systems.Add(new CameraSystem());
            EntityAdmin.Instance.Systems.Add(new GamepadBasicFPSMovementSystem());
            EntityAdmin.Instance.Systems.Add(new SamplerSystem());

            // Create scene objects
            // Order *kinda* matters here: it's the draw order for Shapes

            var camera = Entity.Create("Camera");
            Entity.AddComponent<Transform>(camera).LocalPosition = new Vector3(0, 0, 3f);
            Entity.AddComponent<Camera>(camera);
            Entity.AddComponent<GamepadBasicFPSMovement>(camera);

            //var curlyCircle = Entity.Create("curlyCircle");
            //curlyCircle.AddComponent<Transform>().LocalPosition = new Vector3(0, 0, 0); ;
            //curlyCircle.AddComponent<CurlyCircle>();
            //curlyCircle.AddComponent<Rotate>().Speed = 0.1f;

            AddLine(new Vector3(-1, 1, 0), new Vector3(1, 1, 0));
            AddLine(new Vector3(1, 0, 0), new Vector3(-1, 0, 0));
            AddLine(new Vector3(1, -1, 0), new Vector3(-1, -1, 0));
        }

        static void AddLine(Vector3 start, Vector3 end)
        {
            var line1 = Entity.Create("Line");
            Entity.AddComponent<Transform>(line1);
            var line = Entity.AddComponent<Line>(line1);
            line.Start = start;
            line.End = end;
        }
    }
}
