using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    [RequiresSystem(typeof(PlayerGamepadControlsSystem))]
    public class PlayerGamepadControls : Component
    {
        public float XMin { get; set; } = -31f;
        public float XMax { get; set; } = 31f;
        public float YMin { get; set; } = -10f;
        public float YMax { get; set; } = 25f;

        public float Speed { get; set; } = 50f;
    }
}
