﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public partial class Transform : Component
    {
        public static void AssignParent(Transform child, Transform parent, bool ignoreExceptions = false)
        {
            AssignParent(child, parent, EntityAdmin.Instance, ignoreExceptions);
        }

        public static void AssignParent(Transform child, Transform parent, EntityAdmin admin, bool ignoreExceptions = false)
        {
            var tempParent = parent;
            while (tempParent != null)
            {
                if (tempParent == child)
                {
                    if (ignoreExceptions)
                    {
                        return;
                    }
                    throw new Exception("Can't assign child to parent because child is already a parent of child!");
                }
                tempParent = tempParent.Parent;
            }

            if (child.Parent != null)
            {
                child.Parent.Children.Remove(child);
            }

            if (parent != null)
            {
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
