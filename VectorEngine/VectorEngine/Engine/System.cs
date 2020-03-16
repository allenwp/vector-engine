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
    /// </summary>
    public class System
    {
        public virtual void Tick() { }
    }
}
