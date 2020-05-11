using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public partial class Transform : Component
    {
        public Transform Parent { get; private set; }

        public bool UseThisPSRForWorldTrans { get; set; } = false;

        /// <summary>
        /// Do not modify. Use Transform.AssignParent instead!
        /// </summary>
        public ObservableCollection<Transform> Children { get; private set; }

        public Transform()
        {
            Children = new ObservableCollection<Transform>();
        }

        // Should this be in a System rather than in a Component(?) I think it belongs here...
        public Matrix LocalWorldTransform
        {
            get
            {
                return Matrix.CreateScale(LocalScale) * Matrix.CreateFromQuaternion(LocalRotation) * Matrix.CreateTranslation(LocalPosition);
            }
        }

        // Should this be in a System rather than in a Component(?) I think it belongs here...
        public Matrix WorldTransform
        {
            get
            {
                if (Parent != null)
                {
                    return LocalWorldTransform * Parent.WorldTransform;
                }
                else
                {
                    return LocalWorldTransform;
                }
            }
        }

        public Quaternion Rotation
        {
            get
            {
                if (Parent != null)
                {
                    Vector3 scale;
                    Quaternion rot;
                    Vector3 pos;
                    if (!(LocalWorldTransform * Parent.WorldTransform).Decompose(out scale, out rot, out pos))
                    {
                        throw new Exception("Could not decompose matrix");
                    }
                    return rot;
                }
                else
                {
                    return LocalRotation;
                }
            }
            //set
            //{
            //    if (Parent != null)
            //    {
            //        throw new NotImplementedException();
            //    }
            //    else
            //    {
            //        LocalRotation = value;
            //    }
            //}
        }
        public Vector3 Position
        {
            get
            {
                if (Parent != null)
                {
                    Vector3 scale;
                    Quaternion rot;
                    Vector3 pos;
                    if (!(LocalWorldTransform * Parent.WorldTransform).Decompose(out scale, out rot, out pos))
                    {
                        throw new Exception("Could not decompose matrix");
                    }
                    return pos;

                    // This also works:
                    //return Vector3.Transform(Vector3.Zero, LocalWorldTransform * Parent.WorldTransform);
                }
                else
                {
                    return LocalPosition;
                }
            }
            //set
            //{
            //    if (Parent != null)
            //    {
            //        throw new NotImplementedException();
            //    }
            //    else
            //    {
            //        LocalPosition = value;
            //    }
            //}
        }
        public Vector3 Scale
        {
            get
            {
                if (Parent != null)
                {
                    Vector3 scale;
                    Quaternion rot;
                    Vector3 pos;
                    if (!(LocalWorldTransform * Parent.WorldTransform).Decompose(out scale, out rot, out pos))
                    {
                        throw new Exception("Could not decompose matrix");
                    }
                    return scale;

                    // It's possible this also works, but I didn't test it.
                    //return Vector3.Transform(Vector3.Zero, LocalWorldTransform * Parent.WorldTransform) - Position;
                }
                else
                {
                    return LocalScale;
                }
            }
            //set
            //{
            //    if (Parent != null)
            //    {
            //        throw new NotImplementedException();
            //    }
            //    else
            //    {
            //        LocalScale = value;
            //    }
            //}
        }

        public Quaternion LocalRotation = Quaternion.Identity;
        public Vector3 LocalPosition = Vector3.Zero;
        public Vector3 LocalScale = Vector3.One;

        #region Temporary Editor Junk
        private float yaw;
        [Category("Editor Accessors")]
        public float LocalYaw { get { return yaw; } set { yaw = value; UpdateLocalRotation(); } }
        private float pitch;
        [Category("Editor Accessors")]
        public float LocalPitch { get { return pitch; } set { pitch = value; UpdateLocalRotation(); } }
        private float roll;
        [Category("Editor Accessors")]
        public float LocalRoll { get { return roll; } set { roll = value; UpdateLocalRotation(); } }
        private void UpdateLocalRotation()
        {
            LocalRotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        [Category("Editor Accessors")]
        public float LocalPositionX { get => LocalPosition.X; set => LocalPosition.X = value; }
        [Category("Editor Accessors")]
        public float LocalPositionY { get => LocalPosition.Y; set => LocalPosition.Y = value; }
        [Category("Editor Accessors")]
        public float LocalPositionZ { get => LocalPosition.Z; set => LocalPosition.Z = value; }

        [Category("Editor Accessors")]
        public float LocalScaleX { get => LocalScale.X; set => LocalScale.X = value; }
        [Category("Editor Accessors")]
        public float LocalScaleY { get => LocalScale.Y; set => LocalScale.Y = value; }
        [Category("Editor Accessors")]
        public float LocalScaleZ { get => LocalScale.Z; set => LocalScale.Z = value; }
        #endregion
    }
}
