using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public abstract class PostProcessorLocal3D : Component
    {
        public delegate void PostProcessLocal3DDelegate(List<Sample3D[]> samples3D, PostProcessorLocal3D postProcessor);

        public abstract PostProcessLocal3DDelegate PostProcessLocal3DFuntion { get; }
    }
}
