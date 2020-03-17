using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public class Entity
    {
        public List<Component> Components = new List<Component>();

        public T AddComponent<T> () where T : Component, new()
        {
            var newComponent = new T();
            newComponent.Entity = this;
            Components.Add(newComponent);
            EntityAdmin.Instance.Components.Add(newComponent);
            return newComponent;
        }

        public T GetComponent<T> () where T : Component
        {
            return Components.Where(comp => comp is T).FirstOrDefault() as T;
        }
    }
}
