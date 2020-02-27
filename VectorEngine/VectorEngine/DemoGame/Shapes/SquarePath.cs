using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class SquarePath : Path
    {
        public override SampledPath GetSampledPath(Matrix transform, float stepScale)
        {
            SampledPath result = new SampledPath();
            result.Samples = new Sample[3];
            return result;
        }

        public  
    }
}
