using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    /// <summary>
    /// Stores game state and has no behaviours.
    /// (Said differnt: Only fields, no methods.)
    /// </summary>
    public class Component
    {
        public string Name { get { return GetType().Name; } }

        /// <summary>
        /// Used for the WPF editor because I don't know how to do stuff yet.
        /// </summary>
        public string EntityName { get => Entity.Name; }

        public bool Enabled = true;
        public bool IsActive { get { return Enabled && Entity.Enabled; } }
        public Entity Entity;

        public override string ToString()
        {
            return Name;
        }
    }
}
