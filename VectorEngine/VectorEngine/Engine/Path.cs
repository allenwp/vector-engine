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
        // TODO: Figure out scene graph, etc.
        // To sample a path, it needs a final transform to turn it into screen space...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform">Final matrix to apply all world and camera transforms.</param>
        /// <param name="stepScale">Scale used to reduce or increase number of samples based on size relative to the screen. This is related to the transform.</param>
        /// <returns></returns>
        public abstract List<Sample[]> GetSampledPaths(Matrix transform, float stepScale);
    }
}
