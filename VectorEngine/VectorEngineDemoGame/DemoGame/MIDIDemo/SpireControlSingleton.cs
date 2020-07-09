using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.DemoGame.DemoGame.MIDIDemo
{
    [RequiresSystem(typeof(SpireControlSystem))]
    public class SpireControlSingleton : Component
    {
        public Vector3 Scale;
        public float RotateSpeed;
        public int CurlCount;
        public float StrobeSpeed;
        public float StrobeScale;
        public bool StrobeEnabled;
    }
}
