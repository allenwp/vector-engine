using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Extras;

namespace VectorEngine.Host.Util
{
    public class EditorSystems
    {
        public static List<ECSSystem> GetEditorSystems()
        {
            var result = new List<ECSSystem>();

            result.Add(new GameTimeSystem());
            result.Add(new GamepadSystem());
            result.Add(new GamepadBasicFPSMovementSystem()); // TODO: Make a different system specific to the editor's camera controls

            // "Draw" systems:
            result.Add(new CameraSystem());
            result.Add(new SamplerSystem());

            return result;
        }
    }
}
