using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine.PostProcessing
{
    public class PostProcessingGroupLocal3D : Component
    {
        public List<PostProcessor3D> PostProcessors = new List<PostProcessor3D>();
    }
}
