using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine
{
    public class Camera : Component
    {
        public enum TypeEnum { Perspective, Orthographic }

        public TypeEnum Type = TypeEnum.Perspective;

        /// <summary>
        /// Only used if Type == TypeEnum.Perspective
        /// </summary>
        public float FoV = MathHelper.ToRadians(60);

        /// <summary>
        /// A size of 2 with a related transform of (0,0,z) will allow drawing
        /// "directly to the screen" with 1:1 coordinate mapping.
        /// Only used if Type == TypeEnum.Orthographic
        /// </summary>
        public float Size = 2f;

        public float NearPlane = 1;
        public float FarPlane = 1000;

        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;

        /// <summary>
        /// Each bit of this represents a filter layer on the camera.
        /// 1 means it will render Shapes on that filter layer.
        /// 0 means it will NOT render Shapes on that filter layer.
        /// </summary>
        public uint Filter = uint.MaxValue;
    }
}
