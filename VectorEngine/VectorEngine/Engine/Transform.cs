using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public class Transform : Component
    {
        /// <summary>
        /// Set this to false if you want to draw "directly to the screen" rather than using a Camera
        /// If false, this doesn't need to be transformed by any camera's view or projection transforms.
        /// It will also always have a fidelity of 1 because it does not use any cameras.
        /// World and viewport transforms will still occur on on these transforms.
        /// </summary>
        public bool Is3D = true;

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
        }

        public Quaternion LocalRotation = Quaternion.Identity;
        public Vector3 LocalPosition = Vector3.Zero;
        public Vector3 LocalScale = Vector3.One;
    }
}
