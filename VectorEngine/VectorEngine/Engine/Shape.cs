using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public abstract class Shape
    {
        /// <summary>
        /// If false, this does not exist as a part of the scene graph and doesn't need to be transformed at all.
        /// </summary>
        public bool Is3D = true;

        // TODO: Figure out scene graph, etc.
        // To sample a Shape, it needs a transform...
        // public Transform (???)
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepScale">Scale used to reduce or increase number of samples based on size relative to the screen. This is related to the worldTransform and camera transforms.</param>
        /// <returns></returns>
        public abstract List<Sample[]> GetSamples(Matrix worldTransform, float stepScale);
    }
}
