using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
            int lineLength = Line.LineLength;

            List<Line> lines = new List<Line>();
            lines.Add(new Line() { Start = new Vector3(-0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            lines.Add(new Line() { Start = new Vector3(0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            lines.Add(new Line() { Start = new Vector3(0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });
            lines.Add(new Line() { Start = new Vector3(-0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });

            lines.Add(new Line() { Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, -0.5f) });
            lines.Add(new Line() { Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, -0.5f) });
            lines.Add(new Line() { Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, -0.5f) });
            lines.Add(new Line() { Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, -0.5f) });

            lines.Add(new Line() { Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });
            lines.Add(new Line() { Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            lines.Add(new Line() { Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            lines.Add(new Line() { Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });

            int bufferCount = 400;
            Sample[][] finalBuffers = new Sample[bufferCount][];
            for (int j = 0; j < bufferCount; j++)
            {
                var worldTransform = Matrix.CreateRotationY(MathHelper.LerpPrecise(0, (float)(Math.PI * 2), j / (float)bufferCount));

                Sample[] finalSamples = new Sample[2048]; // buffer size in presonus universal control
                for (int i = 0; i < lines.Count; i++)
                {
                    SampledPath path = lines[i].GetSampledPath(worldTransform, 1f);
                    Array.Copy(path.Samples, 0, finalSamples, i * lineLength, lineLength);
                }
                finalBuffers[j] = finalSamples;
            }
            ASIOOutput.StartDriver(finalBuffers);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
