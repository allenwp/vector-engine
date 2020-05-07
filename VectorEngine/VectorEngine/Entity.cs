using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public partial class Entity
    {
        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Don't modify this directly, use Entity static util methods instead.
        /// </summary>
        public ObservableCollection<Component> Components { get; private set; }

        private Entity(string name)
        {
            Name = name;
            Components = new ObservableCollection<Component>();
        }

        public T GetComponent<T> (bool includeInactive = false) where T : Component
        {
            return Components.Where(comp => comp is T && (includeInactive ? true : comp.IsActive)).FirstOrDefault() as T;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
