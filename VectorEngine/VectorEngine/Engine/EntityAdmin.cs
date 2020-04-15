using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public class EntityAdmin
    {
        public static EntityAdmin Instance { get; } = new EntityAdmin();

        public List<ECSSystem> Systems = new List<ECSSystem>();
        public List<Component> Components = new List<Component>();

        public void Init()
        {
            CreateSingletons();
        }

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
        public static T CreateSingleton<T>() where T : Component, new()
        {
            var entity = new Entity();
            return entity.AddComponent<T>();
        }

        public SamplerSingleton SingletonSampler { get; private set; }
        public GamepadSingleton SingletonGamepad { get; private set; }

        private void CreateSingletons()
        {
            SingletonSampler = CreateSingleton<SamplerSingleton>();
            SingletonGamepad = CreateSingleton<GamepadSingleton>();
        }
        #endregion
    }
}
