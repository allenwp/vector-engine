using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class GamepadSystem : ECSSystem
    {
        public override void Tick()
        {
            var gamepad = EntityAdmin.Instance.GetComponents<GamepadSingleton>().First();
            gamepad.PreviousGamepadState = gamepad.GamepadState;
            gamepad.GamepadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}
