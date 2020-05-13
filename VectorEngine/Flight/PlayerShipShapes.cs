using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    [RequiresSystem(typeof(PlayerShipShapesSystem))]
    public class PlayerShipShapes : Component
    {
        public float AnimationState { get; set; }
        public float AnimationSpeed { get; set; } = 0f;

        public float ShipLength { get; set; } = 10f;

        public float NoseLength { get; set; } = 0.2f;
        public Vector3 NoseTipShape { get; set; } = new Vector3(0.05f, 0.05f, 0.05f);

        public float BodyLength { get; set; } = 0.2f;
        public Vector3 BodyShape { get; set; } = Vector3.One;

        public Vector3 WingEndShape { get; set; } = new Vector3(5f, 0.1f, 1f);

        public List<Transform> RingShapes = new List<Transform>();
    }
}
