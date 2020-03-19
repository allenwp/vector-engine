using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    /// <summary>
    /// Have behaviours and no game state.
    /// (Said differently: Only methods, no fields.)
    /// Named ECSSystem instead of just System because of name conflicts making things frustrating
    /// </summary>
    public class ECSSystem
    {
        public virtual void Tick() { }
    }
}
