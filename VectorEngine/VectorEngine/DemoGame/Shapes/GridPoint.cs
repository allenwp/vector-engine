using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class GridPoint : Shape
    {
        List<Line> lines = new List<Line>();

        public GridPoint()
        {
            lines.Add(new Line() { Is3D = Is3D, LineLength = 20, Start = new Vector3(-0.05f, 0, 0), End = new Vector3(0.05f, 0, 0) });
            lines.Add(new Line() { Is3D = Is3D, LineLength = 20, Start = new Vector3(0, 0, -0.05f), End = new Vector3(0, 0, 0.05f) });
        }

        public override List<Sample[]> GetSamples(Matrix worldTransform, float fidelity)
        {
            List<Sample[]> samples = new List<Sample[]>();
            foreach (var line in lines)
            {
                samples.AddRange(line.GetSamples(worldTransform, fidelity));
            }

            return samples;
        }
    }
}
