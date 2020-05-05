using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    public class Rotate : Component
    {
        public enum AxisEnum { x = 0, y, z }
        public AxisEnum Axis = AxisEnum.y;
        public float LerpAmount = 0;
        public float Speed = 0.3f;
    }
}
