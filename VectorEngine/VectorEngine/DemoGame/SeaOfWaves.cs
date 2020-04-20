using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class SeaOfWaves : Component
    {
        public float AnimationValue;
        public float AnimationSpeed = 1f;
        public List<List<Shapes.WaveTile>> Waves;
    }
}
