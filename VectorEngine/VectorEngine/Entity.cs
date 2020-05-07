using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class Entity
    {
        public string Name { get; set; }

        /// <summary>
        /// TODO: check to see if there is a transform component and, if so, check parent entity. Maybe introduce the idea of "self enabled"
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Don't modify this directly, use Entity static util methods instead.
        /// </summary>
        public ObservableCollection<Component> Components { get; private set; }

        /// <summary>
        /// Do not use. Create through EntityAdmin instead!
        /// </summary>
        /// <param name="name"></param>
        public Entity(string name)
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
