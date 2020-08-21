using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Host.Util
{
    public class EntityAdminUtil
    {
        public static List<Entity> GetEntities(EntityAdmin admin)
        {
            return GetEntities(admin.Components);
        }

        public static List<Entity> GetEntities(List<Component> components)
        {
            List<Entity> result = new List<Entity>();

            foreach (var component in components)
            {
                if (!result.Contains(component.Entity))
                {
                    result.Add(component.Entity);
                }
            }

            return result;
        }

        public static List<Object> GetNextTickLivingObjects(EntityAdmin admin)
        {
            var result = new List<Object>();
            var nextTickComponents = admin.GetNextTickComponents();
            result.AddRange(nextTickComponents);
            result.AddRange(GetEntities(nextTickComponents));
            return result;
        }

        /// <summary>
        /// Will only return a mapping when not in play mode.
        /// </summary>
        public static Dictionary<Entity, List<int>> GetEntityToComponentGroupsMapping(EntityAdmin admin, out ComponentGroup[] componentGroups)
        {
            Dictionary<Entity, List<int>> entityToComponentGroups = new Dictionary<Entity, List<int>>();
            if (!HostHelper.PlayingGame)
            {
                // Determining component groups is expensive because it involves serializing the object graphs.
                // It also has very little relavence when running in playback mode, so only do this in editor mode.
                componentGroups = admin.GetComponents<ComponentGroup>(true).ToArray();
                for (int i = 0; i < componentGroups.Length; i++)
                {
                    List<Component> components = new List<Component>();
                    Serialization.SerializationHelper.Serialize(componentGroups[i], components);
                    var thisGroupEntities = GetEntities(components);
                    foreach (var thisEntity in thisGroupEntities)
                    {
                        if (!entityToComponentGroups.ContainsKey(thisEntity))
                        {
                            entityToComponentGroups[thisEntity] = new List<int>();
                        }
                        entityToComponentGroups[thisEntity].Add(i);
                    }
                }
            }
            else
            {
                componentGroups = new ComponentGroup[0];
            }
            return entityToComponentGroups;
        }
    }
}
