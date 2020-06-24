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
        public string Name { get => GetType().Name; }

        [EditorHelper.Help("Currently only used by the editor to help layout trees, etc.")]
        public Guid Guid { get; private set; }
        /// <summary>
        /// Used for the WPF editor because I don't know how to do stuff yet.
        /// </summary>
        public string EntityName { get => Entity.Name; }

        public bool SelfEnabled { get; set; } = true;
        public bool IsActive { get { return SelfEnabled && Entity.IsActive; } }

        /// <summary>
        /// Don't modify this directly. It will be handled by the static Entity util methods
        /// </summary>
        public Entity Entity;

        public override string ToString()
        {
            return Name;
        }

        public Component()
        {
            Guid = Guid.NewGuid();
        }
    }
}
