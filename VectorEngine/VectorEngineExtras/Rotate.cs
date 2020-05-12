using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.Extras
{
    [RequiresSystem(typeof(RotateSystem))]
    public class Rotate : Component
    {
        public enum AxisEnum { x = 0, y, z }
        public AxisEnum Axis { get; set; } = AxisEnum.y;
        public float LerpAmount { get; set; } = 0;
        public float Speed { get; set; } = 0.3f;
    }
}
