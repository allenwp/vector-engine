using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequiresSystem : Attribute
    {
        public Type ECSSystem;
        public RequiresSystem(Type ecsSystem)
        {
            ECSSystem = ecsSystem;
        }

        public static bool HasECSSystemForType(Type componentType, EntityAdmin admin)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(componentType);
 
            foreach (Attribute attr in attrs)
            {
                if (attr is RequiresSystem)
                {
                    Type requiredSystem = ((RequiresSystem)attr).ECSSystem;
                    if (admin.Systems.Where(system => system.GetType() == requiredSystem).Count() == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
