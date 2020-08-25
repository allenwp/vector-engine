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
        static string assetsPath;
        public static string AssetsPath
        {
            get
            {
                if (assetsPath == null)
                {
                    assetsPath = Program.GameConfigType.GetMethod("GetAssetsPath").Invoke(null, null) as string;
                }
                return assetsPath;
            }
        }

        static List<ECSSystem> gameSystems = null;
        public static List<ECSSystem> GameSystems
        {
            get
            {
                if (gameSystems == null)
                {
                    gameSystems = Program.GameConfigType.GetMethod("GetGameSystems").Invoke(null, null) as List<ECSSystem>;
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
                    editorSystems = Program.GameConfigType.GetMethod("GetEditorSystems").Invoke(null, null) as List<ECSSystem>;
                }
                return editorSystems;
            }
        }

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

                Scene scene = null;
                if (initialSetup)
                {
                    if (FileLoader.GetTextFileConents(Scene.MAIN_SCENE_FILENAME, out string existingSceneJson, true))
                    {
                        scene = Serialization.SerializationHelper.Deserialize<Scene>(existingSceneJson, null, true);
                    }
                    if (scene == null)
                    {
                        scene = EmptyScene.GetEmptyScene();
                        string sceneJson = Serialization.SerializationHelper.Serialize(scene);
                        FileLoader.SaveTextFile(Scene.MAIN_SCENE_FILENAME, sceneJson);
                    }
                }
                else
                {
                    // We're already all set up, so just save current state of game
                    scene = SaveScene();
                }

                GameLoop.Init(GameSystems, scene.Components);
            }
        }

        public static void StopGame(bool initialSetup)
        {
            if (initialSetup || PlayingGame)
            {
                playingGame = false;
                Program.ClearColor = Program.CLEAR_COLOR_STOPPED;
                Program.MidiState.Clear();

                Scene scene = null;
                if (FileLoader.GetTextFileConents(Scene.MAIN_SCENE_FILENAME, out string sceneJson, true))
                {
                    scene = Serialization.SerializationHelper.Deserialize<Scene>(sceneJson, null, true);
                }
                if (scene == null)
                {
                    scene = EmptyScene.GetEmptyScene();
                }

                EditorUI.SelectedEntityComponent = scene.EditorState.SelectedObject;
                Program.MidiState.LoadState(scene.EditorState.MidiAssignments);

                GameLoop.Init(EditorSystems, scene.Components);
            }
        }

        public static Scene SaveScene()
        {
            Scene scene = new Scene();
            scene.Components = EntityAdmin.Instance.Components;
            scene.EditorState = new EditorHelper.EditorState();
            scene.EditorState.SelectedObject = EditorUI.SelectedEntityComponent;
            scene.EditorState.MidiAssignments = Program.MidiState.SaveState();

            string sceneJson = Serialization.SerializationHelper.Serialize(scene);
            FileLoader.SaveTextFile(Scene.MAIN_SCENE_FILENAME, sceneJson);

            return scene;
        }
    }
}
