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
        public Matrix WorldTransform = Matrix.Identity;

        public virtual List<Sample[]> GetSamples()
        {
            // TODO: actually calculate fidelity
            // Base fidelity should be relative to what the size of the shape would be if Is3D was true (no camera, no WorldTransform)
            return GetSamples(WorldTransform, 1f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fidelity">Kind of like a "dynamic level of detail".
        /// It is a scale used to reduce or increase number of resulting samples based on
        /// what physicsal size the shape will be when it is rendered to the screen.
        /// This is based on the worldTransform and camera transforms.</param>
        /// <returns></returns>
        public abstract List<Sample[]> GetSamples(Matrix worldTransform, float fidelity);
    }
}
