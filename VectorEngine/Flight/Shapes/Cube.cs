using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight.Shapes
{
    public class Cube : Shape
    {
        /// <summary>
        /// Copy of the Shapes.Line component, except not a Component because that causes a big mess
        /// for serialization.
        /// </summary>
        private class Line
        {
            public float BaseFidelity;

            public Vector3 Start;
            public Vector3 End;

            public Line()
            {
                BaseFidelity = 130;
            }

            public Line(float baseFidelity)
            {
                BaseFidelity = baseFidelity;
            }

            public List<Sample3D[]> GetSamples3D(float fidelity)
            {
                List<Sample3D[]> result = new List<Sample3D[]>(1);
                int sampleLength = (int)Math.Round(BaseFidelity * fidelity);

                Sample3D[] sample3DArray = new Sample3D[sampleLength];
                for (int i = 0; i < sampleLength; i++)
                {
                    var point3D = Vector3.Lerp(Start, End, (float)i / (float)(sampleLength - 1));
                    sample3DArray[i].Position = point3D;
                    sample3DArray[i].Brightness = 1f;
                }
                result.Add(sample3DArray);

                return result;
            }
        }

        List<Line> cubeLines;

        public Cube() : this(50) { }
        public Cube(float baseFidelity)
        {
            RecreateLines(baseFidelity);
        }

        public void RecreateLines(float baseFidelity)
        {
            cubeLines = new List<Line>();

            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, 0.5f, 0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, -0.5f, 0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });

            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, -0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, -0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, -0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, -0.5f) });

            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, 0.5f, -0.5f), End = new Vector3(-0.5f, 0.5f, 0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, 0.5f, -0.5f), End = new Vector3(0.5f, 0.5f, 0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(0.5f, -0.5f, -0.5f), End = new Vector3(0.5f, -0.5f, 0.5f) });
            cubeLines.Add(new Line(baseFidelity) { Start = new Vector3(-0.5f, -0.5f, -0.5f), End = new Vector3(-0.5f, -0.5f, 0.5f) });
        }

        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> samples = new List<Sample3D[]>();
            foreach (var line in cubeLines)
            {
                samples.AddRange(line.GetSamples3D(fidelity));
            }

            return samples;
        }
    }
}
