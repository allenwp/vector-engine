using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veldrid.MetalBindings;

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

        static List<ECSSystem> editorSystems = null;
        public static List<ECSSystem> EditorSystems
        {
            get
            {
                if (editorSystems == null)
                {
                    editorSystems = Program.GameInitType.GetMethod("GetEditorSystems").Invoke(null, null) as List<ECSSystem>;
                }
                return editorSystems;
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
                    // TODO: get rid of this initial setup altogether. this should already be loaded from a scene or default scene if it fails...

                    // TODO: Correctly load game components from serialization engine. Load default scene only if this fails.
                    components = EmptyScene.GetEmptyScene().Components;
                }
                else
                {
                    components = EntityAdmin.Instance.Components;
                }

                Scene scene = new Scene();
                scene.Components = components;
                scene.EditorState = new EditorHelper.EditorState();
                scene.EditorState.SelectedObject = EditorUI.SelectedEntityComponent;
                scene.EditorState.MidiAssignments = Program.MidiState.SaveState();
                lastJsonSerialization = Serialization.SerializationHelper.Serialize(scene);

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
                Program.MidiState.Clear();

                Scene scene;
                if (lastJsonSerialization != null)
                {
                    scene = Serialization.SerializationHelper.Deserialize<Scene>(lastJsonSerialization);
                }
                else
                {
                    scene = EmptyScene.GetEmptyScene();
                }

                EditorUI.SelectedEntityComponent = scene.EditorState.SelectedObject;
                Program.MidiState.LoadState(scene.EditorState.MidiAssignments);

                GameLoop.Init(EditorSystems, scene.Components);
            }
        }
    }
}
