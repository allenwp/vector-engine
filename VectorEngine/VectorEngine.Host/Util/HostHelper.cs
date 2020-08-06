﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Host.Util
{
    public class HostHelper
    {
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.All
        };

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

                lastJsonSerialization = JsonConvert.SerializeObject(EntityAdmin.Instance.Components, Formatting.Indented, JsonSettings);

                // Temp debug:
                File.WriteAllText("runtimeTempJsonSerialization.txt", lastJsonSerialization);

                // TODO: Correctly load game components from serialization engine. Load default scene only if this fails.
                // Also: set up MIDI controls only the first time you load a scene somehow.

                GameLoop.Init(GameSystems, EntityAdmin.Instance.Components);
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
                    components = JsonConvert.DeserializeObject<List<Component>>(lastJsonSerialization, JsonSettings);
                }
                else
                {
                    components = DefaultScene.GetDefaultScene().Components;
                }

                GameLoop.Init(systems, components);
            }
        }
    }
}
