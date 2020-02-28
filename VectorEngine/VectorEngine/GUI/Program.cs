using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorEngine.DemoGame.Shapes;
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
            Line line = new Line() { Start = new Vector3(0, 0.5f, 0), End = new Vector3(0, -0.5f, 0) };
            line.GetSampledPath(Matrix.Identity, 1f);
            FrameOutput output = new FrameOutput();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
