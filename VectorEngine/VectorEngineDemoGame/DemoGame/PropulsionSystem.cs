using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    public class PropulsionSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach ((var transform, var propulsion) in EntityAdmin.Instance.GetTuple<Transform, Propulsion>())
            {
                transform.LocalPosition += Vector3.Transform(Vector3.Forward * (propulsion.Speed * GameTime.LastFrameTime), transform.LocalRotation);
            }
        }
    }
}
