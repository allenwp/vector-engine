using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    /// <summary>
    /// TODO: make this a billion times better 😂
    /// </summary>
    public class Camera
    {
        public static Vector3 Position = new Vector3(0, 0, 2);
        public static Vector3 Target = new Vector3(0, 0, 0);
        public static Vector3 Up = new Vector3(0, 1, 0);

        public static float FoV = MathHelper.ToRadians(60);
        public static float AspectRatio = 1; // TODO: Allow this to be configured! This needs a test scren first.
        public static float NearPlane = 1;
        public static float FarPlane = 1000;

        public static Matrix ViewMatrix()
        {
            return Matrix.CreateLookAt(Position, Target, Up);
        }

        public static Matrix ProjectionMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(FoV, AspectRatio, NearPlane, FarPlane);
        }
    }
}
