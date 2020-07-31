using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                // TODO: Correctly load game components from serialization engine. Load default scene only if this fails.
                // Also: set up MIDI controls only the first time you load a scene somehow.

                GameLoop.Init(GameSystems, DefaultScene.GetDefaultScene().Components);
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

                GameLoop.Init(systems, DefaultScene.GetDefaultScene().Components);
            }
        }
    }
}
