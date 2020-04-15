using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    public abstract class Shape : Component
    {
        /// <summary>
        /// Only the highest Layer of Shapes will be drawn on a given frame.
        /// This allows for special effects that blank out all other shapes.
        /// </summary>
        public int Layer = 0;

        /// <summary>
        /// Each bit of this represents a filter layer that will be used by Cameras.
        /// Set a bit to 1 to be active on that filter layer.
        /// Set a bit to 0 to be inactive and not rendered on that filter layer.
        /// Shapes will only be drawn once if any of the filter layers are active on
        /// a given camera.
        /// </summary>
        public uint CameraFilterLayers = 0x1;

        /// <summary>
        /// TODO: Maybe this is just a function pointer to a shape function from a different library?
        /// Each shape would need to know how to provide its state onto the shape function, though...
        /// </summary>
        /// <param name="fidelity">Kind of like a "dynamic level of detail".
        /// It is a scale used to reduce or increase number of resulting samples based on
        /// what physicsal size the shape will be when it is rendered to the screen.
        /// This is based on the worldTransform and camera transforms.</param>
        /// <returns></returns>
        public abstract List<Sample3D[]> GetSamples3D(float fidelity);
    }
}
