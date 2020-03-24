using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Engine
{
    public class Camera : Component
    {
        public float FoV = MathHelper.ToRadians(60);
        public float NearPlane = 1;
        public float FarPlane = 1000;

        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;
    }
}
