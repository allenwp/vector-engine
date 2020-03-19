using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.DemoGame
{
    public class Rotate
    {
        public enum AxisEnum { x, y, z}

        public AxisEnum Axis = AxisEnum.y;

        public float LerpAmount = 0;
    }
}
