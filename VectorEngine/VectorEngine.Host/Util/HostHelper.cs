using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            // Maintains object references, obviously
            PreserveReferencesHandling = PreserveReferencesHandling.All,

            // Handles certain cases of circular references
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,

            // Allows inheritance to be correctly deserialized to original subclasses
            TypeNameHandling = TypeNameHandling.All,

            // Not needed with [JsonObject(MemberSerialization.Fields)], but convenient for avoiding public constructors by having a private parameterless constructor
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
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

                JsonSettings.TraceWriter = new MemoryTraceWriter() { LevelFilter = System.Diagnostics.TraceLevel.Warning };
                lastJsonSerialization = JsonConvert.SerializeObject(EntityAdmin.Instance.Components, Formatting.Indented, JsonSettings);
                Console.WriteLine(JsonSettings.TraceWriter);

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
                    JsonSettings.TraceWriter = new MemoryTraceWriter() { LevelFilter = System.Diagnostics.TraceLevel.Warning };
                    components = JsonConvert.DeserializeObject<List<Component>>(lastJsonSerialization, JsonSettings);
                    Console.WriteLine(JsonSettings.TraceWriter);
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
