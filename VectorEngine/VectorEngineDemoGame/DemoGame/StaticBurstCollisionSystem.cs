using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.DemoGame.PostProcessing;

namespace VectorEngine.DemoGame
{
    public class StaticBurstCollisionSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var (transform, burstCollision) in EntityAdmin.Instance.GetTuple<Transform, StaticBurstCollision>())
            {
                if (Vector3.Distance(transform.Position, burstCollision.Player.Position) < burstCollision.DistanceFromPlayer)
                {
                    foreach(var staticPP in EntityAdmin.Instance.GetComponents<StaticPostProcessor>())
                    {
                        staticPP.Amount = burstCollision.StaticAmount;
                    }
                }
            }
        }
    }
}
