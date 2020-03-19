using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public class GamepadSystem : ECSSystem
    {
        public override void Tick()
        {
            EntityAdmin.Instance.SingletonGamepad.GamepadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}
