using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Calibration.Shapes
{
    public class CorneredSquare : Shape
    {
        public float BaseFidelity = 100f;
        public float LineLength = 0.6f;

        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>();

            // Top left
            Sample3D[] line1 = GetLineSegment(new Vector3(-1f, 1f - LineLength, 0f), new Vector3(-1f, 1f, 0f), fidelity);
            Sample3D[] line2 = GetLineSegment(new Vector3(-1f, 1f, 0f), new Vector3(-1f + LineLength, 1f, 0f), fidelity);
            result.Add(CreateCorner(line1, line2, new Vector3(-1f + LineLength, 1f, 0f)));

            // Top right
            line1 = GetLineSegment(new Vector3(1f - LineLength, 1f, 0f), new Vector3(1f, 1f, 0f), fidelity);
            line2 = GetLineSegment(new Vector3(1f, 1f, 0f), new Vector3(1f, 1f - LineLength, 0f), fidelity);
            result.Add(CreateCorner(line1, line2, new Vector3(1f, 1f - LineLength, 0f)));

            // Bottom right
            line1 = GetLineSegment(new Vector3(1f, -1f + LineLength, 0f), new Vector3(1f, -1f, 0f), fidelity);
            line2 = GetLineSegment(new Vector3(1f, -1f, 0f), new Vector3(1f - LineLength, -1f, 0f), fidelity);
            result.Add(CreateCorner(line1, line2, new Vector3(1f - LineLength, -1f, 0f)));

            // Bottom left
            line1 = GetLineSegment(new Vector3(-1f + LineLength, -1f, 0f), new Vector3(-1f, -1f, 0f), fidelity);
            line2 = GetLineSegment(new Vector3(-1f, -1f, 0f), new Vector3(-1f, -1f + LineLength, 0f), fidelity);
            result.Add(CreateCorner(line1, line2, new Vector3(-1f, -1f + LineLength, 0f)));

            return result;
        }

        private Sample3D[] CreateCorner(Sample3D[] first, Sample3D[] second, Vector3 last)
        {
            Sample3D[] result = new Sample3D[first.Length + second.Length + 1];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);
            result[result.Length - 1] = new Sample3D() { Position = last, Brightness = 1f };
            return result;
        }

        private Sample3D[] GetLineSegment(Vector3 inclusiveStart, Vector3 exclusiveEnd, float fidelity)
        {
            int numSamples = (int)(BaseFidelity * fidelity);
            if (numSamples < 1)
            {
                numSamples = 1;
            }

            Sample3D[] result = new Sample3D[numSamples];
            for (int i = 0; i < result.Length; i++)
            {
                result[i].Position = Vector3.Lerp(inclusiveStart, exclusiveEnd, (float)i / numSamples);
                result[i].Brightness = 1f;
            }
            return result;
        }
    }
}
