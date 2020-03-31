using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    /// <summary>
    /// Stores game state and has no behaviours.
    /// (Said differnt: Only fields, no methods.)
    /// </summary>
    public class Component
    {
        public bool Enabled = true;
        public bool IsActive { get { return Enabled && Entity.Enabled; } }
        public Entity Entity;
    }
}
