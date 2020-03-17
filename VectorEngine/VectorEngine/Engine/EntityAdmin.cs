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

        public List<System> Systems = new List<System>();
        public List<Component> Components = new List<Component>();

        public IEnumerable<SamplerTuple> GetSamplerTuples()
        {
            var tuples = new Dictionary<Entity, SamplerTuple>();

            foreach (var component in Components.Where( comp => comp is Transform ))
            {
                if(!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = new SamplerTuple();
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Transform != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Transform = (Transform)component;
                tuples[component.Entity] = tuple;
            }

            foreach (var component in Components.Where(comp => comp is Shape))
            {
                if (!tuples.ContainsKey(component.Entity))
                {
                    tuples[component.Entity] = new SamplerTuple();
                }

                var tuple = tuples[component.Entity];
                if (tuples[component.Entity].Shape != null)
                {
                    throw new Exception("It appears as if an entity has two components of the same type!");
                }
                tuple.Shape = (Shape)component;
                tuples[component.Entity] = tuple;
            }

            // Remove the tuples that don't fit all the requriements:
            tuples.RemoveAll((key, value) => value.Shape == null);
            tuples.RemoveAll((key, value) => value.Transform == null);

            return tuples.Values.ToList();
        }

        #region Singleton Components (System States)
        public SingletonSampler SingletonSampler { get; } = new SingletonSampler();
        #endregion
    }
}
