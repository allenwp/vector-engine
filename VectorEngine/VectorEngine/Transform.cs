using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public partial class Transform : Component
    {
        public Transform Parent { get; private set; }

        /// <summary>
        /// Do not modify. Use Transform.AssignParent instead!
        /// </summary>
        public ObservableCollection<Transform> Children { get; private set; }

        public Transform()
        {
            Children = new ObservableCollection<Transform>();
        }

        // Should this be in a System rather than in a Component(?) I think it belongs here...
        public Matrix WorldTransform
        {
            get
            {
                return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            }
        }

        public Quaternion Rotation
        {
            get
            {
                if (Parent != null)
                {
                    return LocalRotation;
                    throw new NotImplementedException();
                }
                else
                {
                    return LocalRotation;
                }
            }
            set
            {
                if (Parent != null)
                {
                    LocalRotation = value;
                    throw new NotImplementedException();
                }
                else
                {
                    LocalRotation = value;
                }
            }
        }
        public Vector3 Position
        {
            get
            {
                if (Parent != null)
                {
                    return LocalPosition;
                    throw new NotImplementedException();
                }
                else
                {
                    return LocalPosition;
                }
            }
            set
            {
                if (Parent != null)
                {
                    LocalPosition = value;
                    throw new NotImplementedException();
                }
                else
                {
                    LocalPosition = value;
                }
            }
        }
        public Vector3 Scale
        {
            get
            {
                if (Parent != null)
                {
                    return LocalScale;
                    throw new NotImplementedException();
                }
                else
                {
                    return LocalScale;
                }
            }
            set
            {
                if (Parent != null)
                {
                    LocalScale = value;
                    throw new NotImplementedException();
                }
                else
                {
                    LocalScale = value;
                }
            }
        }

        public Quaternion LocalRotation = Quaternion.Identity;
        public Vector3 LocalPosition = Vector3.Zero;
        public Vector3 LocalScale = Vector3.One;
    }
}
