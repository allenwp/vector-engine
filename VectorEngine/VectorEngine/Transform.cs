using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class Transform : Component
    {
        public Transform Parent = null;
        public List<Transform> Children = new List<Transform>();

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
