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

        public static bool HasECSSystemForType(Type componentType, List<ECSSystem> systems, out Type firstMissingSystem)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(componentType);

            firstMissingSystem = null;

            foreach (Attribute attr in attrs)
            {
                if (attr is RequiresSystem)
                {
                    Type requiredSystem = ((RequiresSystem)attr).ECSSystem;
                    if (systems.Where(system => system.GetType() == requiredSystem).Count() == 0)
                    {
                        firstMissingSystem = requiredSystem;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
