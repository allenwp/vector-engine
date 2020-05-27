using System;
using System.Threading;
using System.Windows.Forms;
using VectorEngine;

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
            GameLoop.SceneInit = VectorEngine.DemoGame.SceneEditorTest.Init;
            Thread thread = new Thread(new ThreadStart(GameLoop.Loop));
            thread.Name = "Game Loop Thread";
            thread.IsBackground = true; // So that it aborts along with the application
            thread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
