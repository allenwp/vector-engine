using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    [RequiresSystem(typeof(GamepadSystem))]
    public class GamepadSingleton : Component
    {
        [Newtonsoft.Json.JsonIgnore]
        public GamePadState PreviousGamepadState;
        [Newtonsoft.Json.JsonIgnore]
        public GamePadState GamepadState;

        public bool IsButtonPressed(Buttons button)
        {
            return this.GamepadState.IsButtonDown(button) && PreviousGamepadState.IsButtonUp(button);
        }

        public bool IsButtonReleased(Buttons button)
        {
            return this.GamepadState.IsButtonUp(button) && PreviousGamepadState.IsButtonDown(button);
        }
    }
}
