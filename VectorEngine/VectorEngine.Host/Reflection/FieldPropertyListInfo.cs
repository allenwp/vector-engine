using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Host.Reflection
{
    /// <summary>
    /// Adapter class for either FieldInfo or ProperInfo for a given object
    /// object can also be an IList with special ways of getting and setting values.
    /// </summary>
    public class FieldPropertyListInfo
    {
        object obj;
        FieldInfo fieldInfo = null;
        PropertyInfo propertyInfo = null;

        IList list;
        int listIndex;

        public FieldPropertyListInfo(FieldInfo info, object obj)
        {
            fieldInfo = info;
            this.obj = obj;
        }
        public FieldPropertyListInfo(PropertyInfo info, object obj)
        {
            propertyInfo = info;
            this.obj = obj;
        }
        public FieldPropertyListInfo(IList list, int index)
        {
            this.list = list;
            listIndex = index;
        }

        public MemberInfo MemberInfo
        {
            get
            {
                if (fieldInfo != null)
                {
                    return fieldInfo as MemberInfo;
                }
                else if (propertyInfo != null)
                {
                    return propertyInfo as MemberInfo;
                }
                else
                {
                    return null;
                }
            }
        }

        public Type FieldPropertyType
        {
            get
            {
                if (fieldInfo != null)
                {
                    return fieldInfo.FieldType;
                }
                else if (propertyInfo != null)
                {
                    return propertyInfo.PropertyType;
                }
                else if (list != null)
                {
                    return list.GetType().GetGenericArguments().First();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public string Name
        {
            get
            {
                if (fieldInfo != null)
                {
                    return fieldInfo.Name;
                }
                else if (propertyInfo != null)
                {
                    return propertyInfo.Name;
                }
                else if (list != null)
                {
                    return $"Item {listIndex}";
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public object GetValue()
        {
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }
            else if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj);
            }
            else if (list != null)
            {
                return list[listIndex];
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void SetValue(object value)
        {
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }
            else if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
            else if (list != null)
            {
                list[listIndex] = value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            if (list != null)
            {
                // attributes aren't supported for list items
                return new object[0];
            }
            else
            {
                return fieldInfo != null ? fieldInfo.GetCustomAttributes(inherit) : propertyInfo.GetCustomAttributes(inherit);
            }
        }
    }
}
