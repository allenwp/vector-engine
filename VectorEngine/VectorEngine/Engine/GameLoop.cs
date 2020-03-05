using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

            for(int i = 0; i < 2; i++)
            {
                var cap = GamePad.GetCapabilities(0);
            }

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

                var gamePadState = GamePad.GetState(PlayerIndex.One);
                UpdateCamera();

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

            lerpAmount += 0.001f;
            if (lerpAmount > 1f)
            {
                lerpAmount -= 1f;
            }
            var worldTransform = Matrix.CreateRotationY(MathHelper.LerpPrecise(0, (float)(Math.PI * 2), lerpAmount));

            List<Sample[]> sampledPaths = new List<Sample[]>();
            foreach (var line in lines)
            {
                sampledPaths.AddRange(line.GetSampledPaths(worldTransform, 1f));
            }

            int destinationIndex = 0;
            foreach (var samples in sampledPaths)
            {
                Array.Copy(samples, 0, frameBuffer, destinationIndex, samples.Length);
                destinationIndex += samples.Length;
            }
        }

        static void UpdateCamera()
        {
            var camPos = Camera.Position;

            var gamePadState = GamePad.GetState(PlayerIndex.One);

            camPos.X += gamePadState.ThumbSticks.Left.X * 0.01f;
            camPos.Z -= gamePadState.ThumbSticks.Left.Y * 0.01f;

            Camera.Position = camPos;
            Camera.Target = camPos + Vector3.Forward;
        }
    }
}
