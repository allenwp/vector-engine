using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngineConsole
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
            }
        }
    }
}
