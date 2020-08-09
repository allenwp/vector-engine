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
    }
}
