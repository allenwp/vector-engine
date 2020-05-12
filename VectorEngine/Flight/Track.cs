using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    [RequiresSystem(typeof(TrackSystem))]
    public class Track : Component
    {
        public Transform TrackPoint { get; set; }
        public float Progress { get; set; }
        public float BaseSpeed { get; set; } = 100f;
    }
}
