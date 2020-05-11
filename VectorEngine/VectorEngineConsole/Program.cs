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
        [STAThread]
        static void Main(string[] args)
        {
            // this apartment state is required for the ASIOOutput.StartDriver method
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            GameLoop.SceneInit = VectorEngine.DemoGame.SceneRotatingCubesAndGridPoints.Init;
            GameLoop.Loop();
        }
    }
}
