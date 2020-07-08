using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.Extras
{
    [RequiresSystem(typeof(GamepadBasicFPSMovementSystem))]
    [RequiresSystem(typeof(GamepadSystem))]
    public class GamepadBasicFPSMovement : Component
    {
        public bool UseRealTime = false;

        public float TranslateSpeed = 1.5f;
        public float RotateSpeed = 1f;
        public float Yaw = 0f;
        public float Pitch = 0f;
        public float Roll = 0f;
    }
}
