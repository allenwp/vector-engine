using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine.PostProcessing
{
    public class PostProcessingGroupLocal3D : Component
    {
        // Explicit list instead of having the SamplerSystem just look up all PostProcessor3D components,
        // because some of those components might not be for the Local space, etc.
        public List<PostProcessor3D> PostProcessors = new List<PostProcessor3D>();
    }
}
