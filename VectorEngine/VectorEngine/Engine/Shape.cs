using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public abstract class Shape
    {
        /// <summary>
        /// If false, this does not exist as a part of the scene graph and doesn't need to be transformed at all.
        /// </summary>
        public bool Is3D = true;

        // TODO: Figure out scene graph, etc.
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

        public virtual List<Sample[]> GetSamples()
        {
            float fidelity = 1f;

            if(Is3D)
            {
                float distanceFromCamera = Math.Abs(Vector3.Distance(Position, Camera.Position));
                // Fidelity is relative to the "non-3D" plane equivalent being the distance where half of the camera's FoV shows 1 unit on an axis,
                // which matches the coordinate space that this engine uses, which is -1 to 1 (or 1 unit for half the screen)
                // For a fidelity of 1, the distance from camera must equal (1 / (tan(FoV / 2))) ("TOA" triginometry formula)
                // The first 1 in the following equation is based on half of the camera's vision being 1 unit of screen space ("TOA" triginometry formula)
                // This formula uses the half of the camera's vision being 1 unit to match up with drawing the shape as non-3D
                fidelity = 1f / (distanceFromCamera * (float)Math.Tan(Camera.FoV / 2f));
                fidelity *= (Scale.X + Scale.Y + Scale.Z) / 3f; // Multiply fidelity by average scale
            }

            return GetSamples(WorldTransform, fidelity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fidelity">Kind of like a "dynamic level of detail".
        /// It is a scale used to reduce or increase number of resulting samples based on
        /// what physicsal size the shape will be when it is rendered to the screen.
        /// This is based on the worldTransform and camera transforms.</param>
        /// <returns></returns>
        public abstract List<Sample[]> GetSamples(Matrix worldTransform, float fidelity);
    }
}
