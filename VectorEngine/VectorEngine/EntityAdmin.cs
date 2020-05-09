using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class EntityAdmin
    {
        public static EntityAdmin Instance { get; } = new EntityAdmin();

        public ObservableCollection<ECSSystem> Systems = new ObservableCollection<ECSSystem>();
        public List<Component> Components = new List<Component>();

        /// <summary>
        /// DO NOT USE. Only for use by the editor.
        /// </summary>
        public ObservableCollection<Entity> Entities = new ObservableCollection<Entity>();
        /// <summary>
        /// DO NOT USE. Only for use by the editor.
        /// </summary>
        public ObservableCollection<Transform> RootTransforms = new ObservableCollection<Transform>();

        public void Init()
        {
            CreateSingletons();
        }

        #region Creation and Destruction of Entities and Components
        List<Component> componentsToAdd = new List<Component>();
        List<Component> componentsToRemove = new List<Component>();

        public Entity CreateEntity(string name)
        {
            var result = new Entity(name);
            Entities.Add(result);
            return result;
        }

        public void DestroyEntity(Entity entity)
        {
            var transform = entity.GetComponent<Transform>(true);
            if (transform != null)
            {
                foreach (var child in transform.Children)
                {
                    DestroyEntity(child.Entity);
                }
            }

            var componentsToRemove = entity.Components.ToArray();
            foreach (var component in componentsToRemove)
            {
                RemoveComponent(component);
            }
            Entities.Remove(entity);
        }

        public T AddComponent<T>(Entity entity) where T : Component, new()
        {
            if (!RequiresSystem.HasECSSystemForType(typeof(T), this))
            {
                throw new Exception("A required system for type " + typeof(T) + " is not in the EntityAdmin's system list!");
            }

            var newComponent = new T();
            newComponent.Entity = entity;
            entity.Components.Add(newComponent);
            // Transforms are a special case that are used in the editor, etc.
            var transform = newComponent as Transform;
            if (transform as Transform != null)
            {
                RootTransforms.Add(transform);
            }
            componentsToAdd.Add(newComponent);
            return newComponent;
        }

        public void RemoveComponent<T>(Entity entity) where T : Component, new()
        {
            RemoveComponent(entity.GetComponent<T>(true));
        }

        public void RemoveComponent(Component component)
        {
            componentsToRemove.Add(component);
        }

        /// <summary>
        /// To be called once per frame.
        /// </summary>
        public void AddQueuedComponents()
        {
            foreach (var component in componentsToAdd)
            {
                Components.Add(component);
            }
            componentsToAdd.Clear();
        }

        /// <summary>
        /// To be called once per frame.
        /// </summary>
        public void RemoveQueuedComponents()
        {
            foreach (var component in componentsToRemove)
            {
                var transform = component as Transform;
                if (transform as Transform != null)
                {
                    foreach (var child in transform.Children)
                    {
                        Transform.AssignParent(child, null);
                    }
                    if (transform.Parent == null)
                    {
                        RootTransforms.Remove(transform);
                    }
                }

                Components.Remove(component);
                component.Entity.Components.Remove(component);
                component.Entity = null;
            }
            componentsToRemove.Clear();
        }
        #endregion

        #region Tuples
        public IEnumerable<T1> GetComponents<T1>(bool includeInactive = false) where T1 : Component
        {
            var result = new Dictionary<Entity, T1>();

            foreach (var component in Components.Where(comp => comp is T1 && (includeInactive ? true : comp.IsActive)))
            {
                if (result.ContainsKey(component.Entity))
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                result[component.Entity] = (T1)component;
            }

            return result.Values.ToList();
        }

        public IEnumerable<(T1, T2)> GetTuple<T1, T2>(bool includeInactive = false) where T1 : Component where T2 : Component
        {
            var tuples = new Dictionary<Entity, (T1, T2)>();

            foreach (var component in Components.Where(comp => comp is T1 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item1 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item1 = (T1)component;
                tuples[component.Entity] = tuple;
            }

            foreach (var component in Components.Where(comp => comp is T2 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item2 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item2 = (T2)component;
                tuples[component.Entity] = tuple;
            }

            // Remove the tuples that don't fit all the requriements:
            tuples.RemoveAll((key, value) => value.Item1 == null);
            tuples.RemoveAll((key, value) => value.Item2 == null);

            return tuples.Values.ToList();
        }

        public IEnumerable<(T1, T2, T3)> GetTuple<T1, T2, T3>(bool includeInactive = false) where T1 : Component where T2 : Component where T3 : Component
        {
            var tuples = new Dictionary<Entity, (T1, T2, T3)>();

            foreach (var component in Components.Where(comp => comp is T1 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item1 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item1 = (T1)component;
                tuples[component.Entity] = tuple;
            }

            foreach (var component in Components.Where(comp => comp is T2 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item2 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item2 = (T2)component;
                tuples[component.Entity] = tuple;
            }

            foreach (var component in Components.Where(comp => comp is T3 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item3 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item3 = (T3)component;
                tuples[component.Entity] = tuple;
            }

            // Remove the tuples that don't fit all the requriements:
            tuples.RemoveAll((key, value) => value.Item1 == null);
            tuples.RemoveAll((key, value) => value.Item2 == null);
            tuples.RemoveAll((key, value) => value.Item3 == null);

            return tuples.Values.ToList();
        }

        public IEnumerable<(T1, T2, T3, T4)> GetTuple<T1, T2, T3, T4>(bool includeInactive = false) where T1 : Component where T2 : Component where T3 : Component where T4 : Component
        {
            var tuples = new Dictionary<Entity, (T1, T2, T3, T4)>();

            foreach (var component in Components.Where(comp => comp is T1 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null, null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item1 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item1 = (T1)component;
                tuples[component.Entity] = tuple;
            }

            foreach (var component in Components.Where(comp => comp is T2 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null, null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item2 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item2 = (T2)component;
                tuples[component.Entity] = tuple;
            }

            foreach (var component in Components.Where(comp => comp is T3 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null, null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item3 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item3 = (T3)component;
                tuples[component.Entity] = tuple;
            }

            foreach (var component in Components.Where(comp => comp is T4 && (includeInactive ? true : comp.IsActive)))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = (null, null, null, null);
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Item4 != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Item4 = (T4)component;
                tuples[component.Entity] = tuple;
            }

            // Remove the tuples that don't fit all the requriements:
            tuples.RemoveAll((key, value) => value.Item1 == null);
            tuples.RemoveAll((key, value) => value.Item2 == null);
            tuples.RemoveAll((key, value) => value.Item3 == null);
            tuples.RemoveAll((key, value) => value.Item4 == null);

            return tuples.Values.ToList();
        }
        #endregion

        #region Singleton Components (System States)
        public static T CreateSingleton<T>(string name) where T : Component, new()
        {
            var entity = EntityAdmin.Instance.CreateEntity(name);
            return EntityAdmin.Instance.AddComponent<T>(entity);
        }

        public SamplerSingleton SingletonSampler { get; private set; }
        public GamepadSingleton SingletonGamepad { get; private set; }

        private void CreateSingletons()
        {
            SingletonSampler = CreateSingleton<SamplerSingleton>("Sampler Singleton");
            SingletonGamepad = CreateSingleton<GamepadSingleton>("Gamepad Singleton");
        }
        #endregion
    }
}
