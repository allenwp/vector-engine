using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class Square : Shape
    {
        List<Line> lines = new List<Line>();

        public Square()
        {
            lines.Add(new Line() { Start = new Vector3(-1f, 1f, 0f), End = new Vector3(1f, 1f, 0f) });
            lines.Add(new Line() { Start = new Vector3(1f, 1f, 0f), End = new Vector3(1f, -1f, 0f) });
            lines.Add(new Line() { Start = new Vector3(1f, -1f, 0f), End = new Vector3(-1f, -1f, 0f) });
            lines.Add(new Line() { Start = new Vector3(-1f, -1f, 0f), End = new Vector3(-1f, 1f, 0f) });
        }

        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> samples = new List<Sample3D[]>();
            foreach (var line in lines)
            {
                samples.AddRange(line.GetSamples3D(fidelity));
            }

            return samples;
        }
    }
}
