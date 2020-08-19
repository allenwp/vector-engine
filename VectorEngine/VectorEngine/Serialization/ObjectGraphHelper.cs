using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Serialization
{
    public class ObjectGraphHelper
    {
        public delegate void OnSerializedComponentDelegate(Component component);

        public static OnSerializedComponentDelegate OnSerializedComponent;
        public static OnSerializedComponentDelegate OnDeserializedComponent;
    }
}
