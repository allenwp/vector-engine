using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public partial class Entity
    {
        public static Entity Create(string name)
        {
            var result = new Entity(name);
            EntityAdmin.Instance.Entities.Add(result);
            return result;
        }

        public static void Destroy(Entity entity)
        {
            var componentsToRemove = entity.Components.ToArray();
            foreach (var component in componentsToRemove)
            {
                RemoveComponent(component);
            }
            EntityAdmin.Instance.Entities.Remove(entity);
        }

        public static T AddComponent<T>(Entity entity) where T : Component, new()
        {
            var newComponent = new T();
            newComponent.Entity = entity;
            entity.Components.Add(newComponent);
            EntityAdmin.Instance.Components.Add(newComponent);
            // Transforms are a special case that are used in the editor, etc.
            var transform = newComponent as Transform;
            if (transform as Transform != null)
            {
                EntityAdmin.Instance.RootTransforms.Add(transform);
            }
            return newComponent;
        }

        public static void RemoveComponent<T>(Entity entity) where T : Component, new()
        {
            RemoveComponent(entity.GetComponent<T>(true));
        }

        public static void RemoveComponent(Component component)
        {
            var transform = component as Transform;
            if (transform as Transform != null)
            {
                EntityAdmin.Instance.RootTransforms.Remove(transform);
            }

            EntityAdmin.Instance.Components.Remove(component);

            component.Entity.Components.Remove(component);
            component.Entity = null;
        }
    }
}
