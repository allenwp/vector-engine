using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class Entity
    {
        public string Name;

        public bool Enabled = true;

        public List<Component> Components = new List<Component>();

        public Entity() : this("Entity") { }
        public Entity(string name)
        {
            Name = name;
        }

        public T AddComponent<T> () where T : Component, new()
        {
            var newComponent = new T();
            newComponent.Entity = this;
            Components.Add(newComponent);
            EntityAdmin.Instance.Components.Add(newComponent);
            return newComponent;
        }

        public T GetComponent<T> (bool includeInactive = false) where T : Component
        {
            return Components.Where(comp => comp is T && (includeInactive ? true : comp.IsActive)).FirstOrDefault() as T;
        }
    }
}
