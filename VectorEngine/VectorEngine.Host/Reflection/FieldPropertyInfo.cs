using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Host.Reflection
{
    /// <summary>
    /// Adapter class for either FieldInfo or ProperInfo
    /// </summary>
    public class FieldPropertyInfo
    {
        FieldInfo fieldInfo = null;
        PropertyInfo propertyInfo = null;
        public FieldPropertyInfo(FieldInfo info)
        {
            fieldInfo = info;
        }
        public FieldPropertyInfo(PropertyInfo info)
        {
            propertyInfo = info;
        }

        public Type FieldPropertyType
        {
            get
            {
                return fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
            }
        }

        public string Name
        {
            get
            {
                return fieldInfo != null ? fieldInfo.Name : propertyInfo.Name;
            }
        }

        public object GetValue(object obj)
        {
            return fieldInfo != null ? fieldInfo.GetValue(obj): propertyInfo.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }
            else
            {
                propertyInfo.SetValue(obj, value);
            }
        }
    }
}
