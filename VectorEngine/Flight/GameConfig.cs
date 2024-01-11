using Flight.PostProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Extras;
using VectorEngine.Extras.PostProcessing;

namespace Flight
{
    public class GameConfig
    {
        public static float GetMaxFramesPerSecond() => 80f;

        public static string GetAssetsPath() => @"../../../Flight/Assets";

        public static List<ECSSystem> GetGameSystems()
        {
            var result = new List<ECSSystem>();

            result.Add(new GameTimeSystem());
            result.Add(new GamepadSystem());
            result.Add(new GamepadBasicFPSMovementSystem());
            result.Add(new PlayerGamepadControlsSystem());

            result.Add(new RotateSystem());
            result.Add(new TrackSystem());
            result.Add(new FieldSystem());
            result.Add(new PlayerShipShapesSystem());
            result.Add(new ShadowSystem());

            result.Add(new StaticBurstCollisionSystem());
            result.Add(new StaticBurstSystem());

            // Post Processing Systems:
            result.Add(new StrobePostProcessorSystem());
            result.Add(new PolarCoordinatesPostProcessorSystem());
            result.Add(new PolarCoordHorizonMaskPostProcessorSystem());
            result.Add(new StaticPostProcessorSystem());

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
