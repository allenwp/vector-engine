using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public abstract class Path
    {
        public abstract SampledPath GetSampledPath(Matrix transform, float stepScale);
    }
}
