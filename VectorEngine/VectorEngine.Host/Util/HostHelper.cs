using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VectorEngine.Host.Util
{
    public class HostHelper
    {
        static List<ECSSystem> gameSystems = null;
        public static List<ECSSystem> GameSystems
        {
            get
            {
                if (gameSystems == null)
                {
                    gameSystems = Program.GameInitType.GetMethod("GetGameSystems").Invoke(null, null) as List<ECSSystem>;
                }
                return gameSystems;
            }
        }

        private static string lastJsonSerialization = null;

        private static bool playingGame = false;
        public static bool PlayingGame
        {
            get
            {
                return playingGame;
            }
            set
            {
                if (value != playingGame)
                {
                    TogglePlayGame();
                }
            }
        }

        public static void TogglePlayGame()
        {
            if (PlayingGame)
            {
                StopGame(false);
            }
            else
            {
                PlayGame(false);
            }
        }

        public static void PlayGame(bool initialSetup)
        {
            if (initialSetup || !PlayingGame)
            {
                playingGame = true;
                Program.ClearColor = Program.CLEAR_COLOR_PLAY;

                List<Component> components;

                if (initialSetup)
                {
                    // TODO: Correctly load game components from serialization engine. Load default scene only if this fails.
                    components = EmptyScene.GetEmptyScene().Components;
                }
                else
                {
                    components = EntityAdmin.Instance.Components;
                }

                lastJsonSerialization = Serialization.SerializationHelper.Serialize(components);

                // Temp debug:
                File.WriteAllText("runtimeTempJsonSerialization.txt", lastJsonSerialization);

                GameLoop.Init(GameSystems, components);
            }
        }

        public static void StopGame(bool initialSetup)
        {
            if (initialSetup || PlayingGame)
            {
                playingGame = false;
                Program.ClearColor = Program.CLEAR_COLOR_STOPPED;

                List<ECSSystem> systems = EditorSystems.GetEditorSystems();

                // TODO: Correctly load game components from serialization engine.
                // Also: set up MIDI controls only the first time you load a scene somehow.

                List<Component> components;
                if (lastJsonSerialization != null)
                {
                    components = Serialization.SerializationHelper.Deserialize<List<Component>>(lastJsonSerialization);
                }
                else
                {
                    components = EmptyScene.GetEmptyScene().Components;
                }

                GameLoop.Init(systems, components);
            }
        }
    }
}
