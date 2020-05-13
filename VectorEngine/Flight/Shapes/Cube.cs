using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight.Shapes
{
    public class Cube : Shape
    {
        List<Line> lines;

        public Cube() : this(50) { }
        public Cube(float baseFidelity)
        {
            RecreateLines(baseFidelity);
        }

        public void RecreateLines(float baseFidelity)
        {
            lines = new List<Line>();

            lines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });

            lines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, -0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, -0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, -0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, -0.5f) });

            lines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            lines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });
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
