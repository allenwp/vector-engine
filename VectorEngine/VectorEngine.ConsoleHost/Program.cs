using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.ConsoleHost
{
    class Program
    {
        [STAThread] // Needed for ASIOOutput.StartDriver method
        static void Main(string[] args)
        {
            GameLoop.Init(VectorEngine.DemoGame.SceneRotatingCubesAndGridPoints.Init);
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
