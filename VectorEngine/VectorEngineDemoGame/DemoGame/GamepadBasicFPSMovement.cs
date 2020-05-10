using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    [RequiresSystem(typeof(GamepadBasicFPSMovementSystem))]
    [RequiresSystem(typeof(GamepadSystem))]
    public class GamepadBasicFPSMovement : Component
    {
        public float TranslateSpeed = 1.5f;
        public float RotateSpeed = 1f;
        public float Yaw = 0f;
        public float Pitch = 0f;
        public float Roll = 0f;
    }
}
