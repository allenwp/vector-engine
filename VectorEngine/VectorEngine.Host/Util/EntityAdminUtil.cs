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
            List<Entity> result = new List<Entity>();

            foreach (var component in admin.Components)
            {
                if (!result.Contains(component.Entity))
                {
                    result.Add(component.Entity);
                }
            }

            return result;
        }
    }
}
