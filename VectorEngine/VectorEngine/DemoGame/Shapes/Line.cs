using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class Line : Path
    {
        public Vector3 Start;
        public Vector3 End;

        public override SampledPath GetSampledPath(Matrix transform, float stepScale)
        {
            SampledPath result = new SampledPath();
            result.Samples = new Sample[(int)Math.Round(100 * stepScale)];
            for(int i = 0; i < result.Samples.Length; i++)
            {
                var point3D = Vector3.Lerp(Start, End, (float)i / (float)result.Samples.Length);

                Vector4 point4D = new Vector4(point3D, 1);
                Vector4 v4 = Vector4.Transform(point4D, transform);
                v4 = Transformer.performViewTransform(point4D, Camera.ViewMatrix());
                v4 = Transformer.performProjectionTransform(v4, Camera.ProjectionMatrix());
                bool clipped = Transformer.clip(v4);
                if(!clipped)
                {
                    Vector2 result2D = Transformer.performViewportTransform(v4);
                    result.Samples[i].X = result2D.X;
                    result.Samples[i].Y = result2D.Y;
                    result.Samples[i].Brightness = 1;
                }
                else
                {
                    if (i > 0)
                    {
                        result.Samples[i] = result.Samples[i - 1];
                        result.Samples[i].Brightness = 0;
                    }
                    else
                    {
                        result.Samples[i].Brightness = 0;
                        // TODO: do this all better so that a different sized array can be returned
                    }
                }
            }
            return result;
        }
    }
}
