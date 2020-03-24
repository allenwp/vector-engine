using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class GamepadLookTranslate : Component
    {
        public float TranslateSpeed = 1.5f;
        public float RotateSpeed = 1f;
        public float Yaw = 0f;
        public float Pitch = 0f;
    }
}
