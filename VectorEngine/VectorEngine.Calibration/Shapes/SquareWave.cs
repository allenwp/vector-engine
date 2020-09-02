using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Calibration.Shapes
{
    /// <summary>
    /// This shape should appear to have square edges if the DisplayProfile.FidelityScale is set correctly.
    /// </summary>
    public class SquareWave : Shape
    {
        enum ScanningState { FirstUpper, SecondUpper, FirstBottom, SecondBottom }

        public float BaseFidelity = 15f;
        public bool BlankLineSegments = false;
        public float StepSize = 0.25f;

        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> blankedPoints = new List<Sample3D[]>();

            List<Vector3> points = new List<Vector3>();
            Vector3 point = new Vector3(FrameOutput.DisplayProfile.AspectRatio * -1f, 1f, 0);
            ScanningState state = ScanningState.FirstUpper;
            bool scanningRight = true;
            while (true)
            {
                points.Add(point);
                switch (state)
                {
                    case ScanningState.FirstUpper:
                        state = ScanningState.SecondUpper;
                        point.X += scanningRight ? StepSize : -1f * StepSize;
                        break;
                    case ScanningState.SecondUpper:
                        state = ScanningState.FirstBottom;
                        point.Y -= StepSize;
                        break;
                    case ScanningState.FirstBottom:
                        state = ScanningState.SecondBottom;
                        point.X += scanningRight ? StepSize : -1f * StepSize;
                        break;
                    case ScanningState.SecondBottom:
                        state = ScanningState.FirstUpper;
                        point.Y += StepSize;
                        break;
                }
                if (point.X > FrameOutput.DisplayProfile.AspectRatio || point.X < FrameOutput.DisplayProfile.AspectRatio * -1f)
                {
                    if (scanningRight)
                    {
                        point.X = FrameOutput.DisplayProfile.AspectRatio;
                    }
                    else
                    {
                        point.X = FrameOutput.DisplayProfile.AspectRatio * -1f;
                    }
                    scanningRight = !scanningRight;
                    points.Add(point);
                    point.Y -= StepSize;
                    points.Add(point);
                    if (state == ScanningState.FirstUpper || state == ScanningState.SecondUpper)
                    {
                        point.Y -= StepSize;
                        points.Add(point);
                    }
                    point.Y -= StepSize;
                }
                if (point.Y < -1f)
                {
                    point.Y = -1f;
                    points.Add(point);
                    break;
                }
            }

            for (int i = 0; i < points.Count() - 1; i++)
            {
                blankedPoints.Add(GetLineSegment(points[i], points[i + 1], fidelity));
            }

            List<Sample3D[]> result;
            if (BlankLineSegments)
            {
                result = blankedPoints;
            }
            else
            {
                result = new List<Sample3D[]>(0);
                int lineSegmentLength = blankedPoints[0].Length;
                Sample3D[] bigArray = new Sample3D[blankedPoints.Count() * lineSegmentLength];
                for (int i = 0; i < blankedPoints.Count(); i++)
                {
                    blankedPoints[i].CopyTo(bigArray, i * lineSegmentLength);
                }
                result.Add(bigArray);
            }
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
