using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace VectorEngine.DemoGame.Shapes
{
    public class GridPoint : Shape
    {
        List<Line> lines = new List<Line>();

        public GridPoint()
        {
            lines.Add(new Line() { BaseFidelity = 20, Start = new Vector3(-0.05f, 0, 0), End = new Vector3(0.05f, 0, 0) });
            lines.Add(new Line() { BaseFidelity = 20, Start = new Vector3(0, 0, -0.05f), End = new Vector3(0, 0, 0.05f) });
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
