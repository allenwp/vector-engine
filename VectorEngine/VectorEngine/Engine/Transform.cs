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
        /// If false, this does not exist as a part of the scene graph and doesn't need to be transformed at all.
        /// </summary>
        public bool Is3D = true;

        // TODO: Figure out scene graph, etc.
        // Should this be in a System rather than in a Component(?) I think it belongs here...
        public Matrix WorldTransform
        {
            get
            {
                return Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);
            }
        }

        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
    }
}
