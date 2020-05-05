using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.PostProcessing
{
    /// <summary>
    /// If applied to an Entity with a Shape, this post-processing will occur in LOCAL space
    /// before the Shape's WorldTransform is applied.
    /// If applied to an Entity with a Camera, this post-processing will occur in WORLD space
    /// to an entire Camera's Sample3D stream before it has been transformed to screen space.
    /// </summary>
    public class PostProcessingGroup3D : Component
    {
        // Explicit list for no good reason other than looking up tuples of multiple post processing effects
        // with the same base class is not supported
        public List<PostProcessor3D> PostProcessors = new List<PostProcessor3D>();
    }
}
