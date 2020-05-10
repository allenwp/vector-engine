using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame
{

    [RequiresSystem(typeof(PropulsionSystem))]
    public class Propulsion : Component
    {
        public float Speed = 10f;
    }
}
