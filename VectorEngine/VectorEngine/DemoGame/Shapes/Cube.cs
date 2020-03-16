using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class Cube : Shape
    {
        List<Line> lines = new List<Line>();
        public Cube()
        {
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(-0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(-0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });
                                  
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, -0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, -0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, -0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, -0.5f) });
                                  
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            lines.Add(new Line() {Is3D = Is3D, Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });
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
