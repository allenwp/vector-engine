using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    [RequiresSystem(typeof(FieldSystem))]
    public class Field : Component
    {
        public float Depth { get; set; } = 500f;
        public float Width { get; set; } = 150f;
        public int SpireObjectCount { get; set; } = 10;
        public List<Transform> Objects;

        public int DotCountX { get; set; } = 6;
        public int DotCountZ { get; set; } = 10;

    }
}
