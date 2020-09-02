using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Extras;
using VectorEngine.Extras.PostProcessing;

namespace VectorEngine.Calibration
{
    public class GameConfig
    {
        public static float GetTargetFramesPerSecond() => float.MaxValue;

        public static string GetAssetsPath() => @"../../../VectorEngine.Calibration/Assets";

        public static List<ECSSystem> GetGameSystems()
        {
            var result = new List<ECSSystem>();

            result.Add(new GameTimeSystem());
            result.Add(new GamepadSystem());
            result.Add(new GamepadBasicFPSMovementSystem());

            result.Add(new RotateSystem());

            result.Add(new DisplayProfileAdjusterSystem());

            // "Draw" systems:
            result.Add(new CameraSystem());
            result.Add(new SamplerSystem());

            return result;
        }

        public static List<ECSSystem> GetEditorSystems()
        {
            var result = new List<ECSSystem>();

            result.Add(new GameTimeSystem());
            result.Add(new GamepadSystem());
            result.Add(new GamepadBasicFPSMovementSystem()); // TODO: Make a different system specific to the editor's camera controls
            result.Add(new DisplayProfileAdjusterSystem());

            // "Draw" systems:
            result.Add(new CameraSystem());
            result.Add(new SamplerSystem());

            return result;
        }
    }
}
