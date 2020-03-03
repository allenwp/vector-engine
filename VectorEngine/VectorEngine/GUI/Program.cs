using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorEngine.DemoGame.Shapes;
using VectorEngine.Engine;
using VectorEngine.Output;

namespace VectorEngineGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread thread = new Thread(new ThreadStart(GameLoop.Loop));
            thread.Name = "Game Loop Thread";
            // this apartment state is required for the ASIOOutput.StartDriver method
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
