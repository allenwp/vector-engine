using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.PostProcessing
{

    /// <summary>
    /// If applied to a Camera, post processing will be applied to this Camera's samples
    /// just after they have been transformed and clipped to screen space.
    /// If applied to a SamplerSingleton, post processing will be applied to ALL Camera's
    /// samples just before they are sent off to the frame buffer.
    /// </summary>
    [RequiresSystem(typeof(SamplerSystem))]
    public class PostProcessingGroup2D : Component
    {
        // Explicit list for no good reason other than looking up tuples of multiple post processing effects
        // with the same base class is not supported
        public List<PostProcessor2D> PostProcessors = new List<PostProcessor2D>();
    }
}
