using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    [RequiresSystem(typeof(StaticBurstSystem))]
    public class StaticBurst : Component
    {
        public float DecaySpeed { get; set; } = 0.5f;
    }
}
