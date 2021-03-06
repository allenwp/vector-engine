﻿using Microsoft.Win32.SafeHandles;
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

        public List<ECSSystem> Systems = new List<ECSSystem>();
        public List<Component> Components = new List<Component>();

        /// <summary>
        /// I only see this being useful for the editor.
        /// Gets a copy of the main Components list, but with the componentsToAdd and without
        /// the componentsToRemove
        /// </summary>
        public List<Component> GetNextTickComponents()
        {
            return Components.Where(comp => !componentsToRemove.Contains(comp)).Union(componentsToAdd).ToList();
        }

        public void Init(List<ECSSystem> systems, List<Component> components)
        {
            Systems = systems;
            Components = components;
        }

        #region Creation and Destruction of Entities and Components
        List<Component> componentsToAdd = new List<Component>();
        List<Component> componentsToRemove = new List<Component>();

        public Entity CreateEntity(string name)
        {
            var result = new Entity(name);
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
        }

        public T AddComponent<T>(Entity entity) where T : Component, new()
        {
            var newComponent = new T();
            newComponent.Entity = entity;
            entity.Components.Add(newComponent);
            componentsToAdd.Add(newComponent);
            return newComponent;
        }

        public Component AddComponent(Entity entity, Type type)
        {
            Component newComponent = (Component)Activator.CreateInstance(type);
            newComponent.Entity = entity;
            entity.Components.Add(newComponent);
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
                if (transform != null)
                {
                    if (transform.Parent != null)
                    {
                        transform.Parent.Children.Remove(transform);
                    }

                    Transform[] children = new Transform[transform.Children.Count()];
                    transform.Children.CopyTo(children);
                    foreach (var child in children)
                    {
                        Transform.AssignParent(child, null);
                    }
                }

                Components.Remove(component);
                component.Entity.Components.Remove(component);
                component.Entity = null;
            }
            componentsToRemove.Clear();
        }

        public static void ClearSceneDefaultAdmin()
        {
            Instance.ClearScene();
        }

        /// <summary>
        /// Destroys all entities that aren't marked with DontDestroyOnClear
        /// </summary>
        public void ClearScene()
        {
            ClearSceneFromComponents(Components);
            ClearSceneFromComponents(componentsToAdd);
        }

        private void ClearSceneFromComponents(List<Component> components)
        {
            foreach (var component in components.Where(comp => !comp.Entity.HasComponent<DontDestroyOnClear>(true)))
            {
                RemoveComponent(component);
            }
        }
        #endregion

        #region Tuples & Components
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
        
        /// <summary>
        /// Helper method to simply create a Componenet and gives it an empty Entity.
        /// </summary>
        public T CreateSingleton<T>(string name) where T : Component, new()
        {
            var entity = CreateEntity(name);
            return AddComponent<T>(entity);
        }
        #endregion
    }
}
