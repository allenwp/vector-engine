using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public partial class Transform : Component
    {
        public static void AssignParent(Transform child, Transform parent)
        {
            if (child.Parent != null)
            {
                child.Parent.Children.Remove(child);
            }

            if (parent == null)
            {
                // Only add it as a root transform if it wasn't already a root transform.
                if (child.Parent != null)
                {
                    EntityAdmin.Instance.RootTransforms.Add(child);
                }
            }
            else
            {
                // Only remove it as a root transform if it was already a root transform.
                if (child.Parent == null)
                {
                    EntityAdmin.Instance.RootTransforms.Remove(child);
                }

                if (parent.Children.Contains(child))
                {
                    Console.WriteLine("WARNING: assigning a child Transform when it is already a child!");
                }
                else
                {
                    parent.Children.Add(child);
                }
            }

            child.Parent = parent;
        }
    }
}
