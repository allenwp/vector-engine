using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    public class GameLoop
    {

        public static void Loop()
        {
            ASIOOutput.StartDriver();

            while (true)
            {
                if ((FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingForBuffer1 && FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.ReadingBuffer1)
                    || (FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingForBuffer2 && FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.ReadingBuffer2))
                {
                    continue;
                }

                Sample[] finalSamples;
                if (FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingForBuffer1)
                {
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WrittingBuffer1;
                    finalSamples = FrameOutput.Buffer1;
                }
                else
                {
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WrittingBuffer2;
                    finalSamples = FrameOutput.Buffer2;
                }

                FrameOutput.ClearBuffer(finalSamples);

                DrawRotatingCube(finalSamples);

                if (FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WrittingBuffer1)
                {
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WaitingForBuffer2;
                }
                else
                {
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WaitingForBuffer1;
                }
            }
        }

        static float lerpAmount = 0;
        static void DrawRotatingCube(Sample[] frameBuffer)
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

            lerpAmount += 0.005f;
            if (lerpAmount > 1f)
            {
                lerpAmount -= 1f;
            }
            var worldTransform = Matrix.CreateRotationY(MathHelper.LerpPrecise(0, (float)(Math.PI * 2), lerpAmount));

            for (int i = 0; i < lines.Count; i++)
            {
                SampledPath path = lines[i].GetSampledPath(worldTransform, 1f);
                Array.Copy(path.Samples, 0, frameBuffer, i * lineLength, lineLength);
            }
        }
    }
}
