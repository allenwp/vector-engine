using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    [RequiresSystem(typeof(StaticBurstCollisionSystem))]
    public class StaticBurstCollision : Component
    {
        public float StaticAmount { get; set; } = 0.2f;
        public float DistanceFromPlayer { get; set; } = 10f;
        public Transform Player;
    }
}
