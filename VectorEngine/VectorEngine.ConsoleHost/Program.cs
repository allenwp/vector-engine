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
        public static readonly Type GameInitType = typeof(Flight.GameInit);
        public static readonly Assembly GameAssembly = Assembly.GetAssembly(GameInitType);

        [STAThread] // Needed for ASIOOutput.StartDriver method
        static void Main(string[] args)
        {
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

            var GameSystems = Program.GameInitType.GetMethod("GetGameSystems").Invoke(null, null) as List<ECSSystem>;
            
            GameLoop.Init(GameSystems, scene.Components);
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
