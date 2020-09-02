using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.ConsoleHost
{
    class Program
    {
        public static readonly Type GameConfigType = typeof(Flight.GameConfig);

        [STAThread] // Needed for ASIOOutput.StartDriver method
        static void Main(string[] args)
        {
            float? targetFramesPerSecond = Program.GameConfigType.GetMethod("GetTargetFramesPerSecond").Invoke(null, null) as float?;
            if (targetFramesPerSecond == null)
            {
                throw new Exception("Cannot GetTargetFramesPerSecond() from GameConfig");
            }

            string assetsPath = Program.GameConfigType.GetMethod("GetAssetsPath").Invoke(null, null) as string;

            FileLoader.Init(assetsPath);
            FileLoader.LoadAllComponentGroups();

            Scene scene;
            if (FileLoader.GetTextFileConents(Scene.MAIN_SCENE_FILENAME, out string sceneJson, true))
            {
                scene = Serialization.SerializationHelper.Deserialize<Scene>(sceneJson, null, true);
            }
            else
            {
                Console.WriteLine("Could not load scene!");
                Console.ReadLine();
                throw new Exception("Could not load scene");
            }

            var GameSystems = Program.GameConfigType.GetMethod("GetGameSystems").Invoke(null, null) as List<ECSSystem>;

            GameLoop.Init(GameSystems, scene.Components, (float)targetFramesPerSecond);
            while (true)
            {
                GameLoop.Tick();
                // Debug test code to simulate tricky double buffer situations
                //if (new Random().Next(60) == 0)
                //{
                //    Console.WriteLine("Sleeping to simulate a long Host time.");
                //    Thread.Sleep(200);
                //}
            }
        }
    }
}
