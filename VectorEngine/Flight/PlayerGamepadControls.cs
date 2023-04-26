using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    [RequiresSystem(typeof(PlayerGamepadControlsSystem))]
    public class PlayerGamepadControls : Component
    {
        public Vector2 max_pitch_yaw = new Vector2(0.4f, 0.4f);
        public float max_rotate_speed = 0.1f;
        public float settle_pitch_yaw_tween_steps = 4f;
        public float translate_speed = 1.0f;
        public float camera_rotation_scale = 0.5f;

        public float track_ground = 0f;
        public Vector2 track_bounds = new Vector2(60, 35);

        public Transform camera_transform;

        //This is based on FOV of the camera and distance of the ship to the camera.
        //Hardcoded rather than calculated based on FOV because this allows FOV to
        //change with visual effects and it's a "tweek until it looks right" thing anyway.
        public Vector2 camera_bounds_reduction = new Vector2(30f, 30f);
        
        public float shoot_cooldown_time = 0.25f;

        public Vector2 ship_pitch_yaw = Vector2.Zero;

        public float shoot_cooldown = 0;
    }
}
