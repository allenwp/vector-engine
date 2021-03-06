﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.EditorHelper;
using VectorEngine.Output;

namespace VectorEngine
{
    [RequiresSystem(typeof(CameraSystem))]
    public class Camera : Component
    {
        public const float MinFoV = 0.001f;
        public const float MaxFoV = (float)Math.PI - 0.001f;

        public enum ProjectionTypeEnum { Perspective, Orthographic }

        public ProjectionTypeEnum ProjectionType = ProjectionTypeEnum.Perspective;

        private float fov = MathHelper.ToRadians(60);
        /// <summary>
        /// Only used if Type == TypeEnum.Perspective
        /// </summary>
        [Range(MinFoV, MaxFoV)]
        public float FoV
        {
            get
            {
                return fov;
            }
            set
            {
                if (value <= MinFoV)
                {
                    value = MinFoV;
                }
                else if (value > MaxFoV)
                {
                    value = MaxFoV;
                }
                fov = value;
            }
        }

        /// <summary>
        /// A size of 2 with a related transform of (0,0,z) will allow drawing
        /// "directly to the screen" with 1:1 coordinate mapping.
        /// Only used if Type == TypeEnum.Orthographic
        /// </summary>
        public float OrthographicSize = 2f;

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

        /// <summary>
        /// Only cameras on the highest Priority will render.
        /// This is used for the Editor camera, but can be used for other effects.
        /// </summary>
        public uint Priority = 0;
    }
}
