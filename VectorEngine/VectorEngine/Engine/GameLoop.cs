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
        static List<Shape> shapes = new List<Shape>();

        static bool firstFrameRendered = false;
        public static void Loop()
        {
            ASIOOutput.StartDriver();

            Init();

            while (true)
            {
                // If we're still waiting for the output to finish reading a buffer, don't do anything else
                if ((FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingForBuffer1 && FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.ReadingBuffer1)
                    || (FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingForBuffer2 && FrameOutput.ReadState == (int)FrameOutput.ReadStateEnum.ReadingBuffer2))
                {
                    // TODO: do something better with thread locks or Parallel library or something that doesn't involve spinning
                    continue;
                }

                // We're no longer waiting on output to finish with its buffer.
                // Progress the FrameOutput state
                FrameOutput.WriteStateEnum writeState;
                if (FrameOutput.WriteState == (int)FrameOutput.WriteStateEnum.WaitingForBuffer1)
                {
                    writeState = FrameOutput.WriteStateEnum.WrittingBuffer1;
                }
                else
                {
                    writeState = FrameOutput.WriteStateEnum.WrittingBuffer2;
                }
                FrameOutput.WriteState = (int)writeState;

                // "Update" the game
                Update();

                // "Draw" the game
                var samples = Draw();

                // Finally, prepare and fill the FrameOutput buffer:

                // Find out how many samples we have in the full set
                int sampleCount = 0;
                foreach (var sampleArray in samples)
                {
                    // TODO: include blanking injection in this count
                    sampleCount += sampleArray.Length;
                }

                // Set up the final buffer with the correct sample length
                // This is variable (variable frame rate based on paramenters in FrameOutput class)
                Sample[] finalBuffer;
                if (sampleCount <= FrameOutput.TARGET_BUFFER_SIZE)
                {
                    finalBuffer = new Sample[FrameOutput.TARGET_BUFFER_SIZE];
                    // Only in this case to we need to clear the buffer. In the other cases we will be filling it entirely
                    FrameOutput.ClearBuffer(finalBuffer);
                }
                else if (sampleCount > FrameOutput.MAX_BUFFER_SIZE)
                {
                    finalBuffer = new Sample[FrameOutput.MAX_BUFFER_SIZE];
                }
                else
                {
                    finalBuffer = new Sample[sampleCount];
                }

                // Copy the full set of samples into the final buffer:
                int destinationIndex = 0;
                foreach (var sampleArray in samples)
                {
                    // TODO: Handle when sampleCount > FrameOutput.MAX_BUFFER_SIZE
                    // In this case, we can't do this copy because the finalBuffer is not large enough.
                    Array.Copy(sampleArray, 0, finalBuffer, destinationIndex, sampleArray.Length);
                    destinationIndex += sampleArray.Length;
                }

                // "Blit" the buffer and progress the frame buffer write state
                if (writeState == FrameOutput.WriteStateEnum.WrittingBuffer1)
                {
                    FrameOutput.Buffer1 = finalBuffer;
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WaitingForBuffer2;
                }
                else
                {
                    FrameOutput.Buffer2 = finalBuffer;
                    FrameOutput.WriteState = (int)FrameOutput.WriteStateEnum.WaitingForBuffer1;
                }
            }
        }

        static void Init()
        {
            shapes.Add(new Cube());
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    GridPoint point = new GridPoint();
                    point.WorldTransform = Matrix.CreateTranslation(i * 0.5f, -0.5f, j * 0.5f);
                    shapes.Add(point);
                }
            }
        }

        static void Update()
        {
            UpdateCubeRotations();
            var gamePadState = GamePad.GetState(PlayerIndex.One);
            UpdateCamera(gamePadState);
        }

        static float lerpAmount = 0;
        static void UpdateCubeRotations()
        {
            foreach (var shape in shapes)
            {
                Cube cube = shape as Cube;
                if (cube != null)
                {
                    lerpAmount += 0.001f;
                    if (lerpAmount > 1f)
                    {
                        lerpAmount -= 1f;
                    }
                    cube.WorldTransform = Matrix.CreateRotationY(MathHelper.LerpPrecise(0, (float)(Math.PI * 2), lerpAmount));
                }
            }
        }

        static void UpdateCamera(GamePadState gamePadState)
        {
            var camPos = Camera.Position;

            camPos.X += gamePadState.ThumbSticks.Left.X * 0.01f;
            camPos.Z -= gamePadState.ThumbSticks.Left.Y * 0.01f;

            Camera.Position = camPos;
            Camera.Target = camPos + Vector3.Forward;
        }

        static List<Sample[]> Draw()
        {
            List<Sample[]> result = new List<Sample[]>();
            foreach(Shape shape in shapes)
            {
                result.AddRange(shape.GetSamples());
            }
            return result;
        }
    }
}
