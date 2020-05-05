using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class FollowSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach ((var transform, var follow) in EntityAdmin.Instance.GetTuple<Transform, Follow>())
            {
                Transform transToFollow = follow.EntityToFollow.GetComponent<Transform>();
                transform.Position = transToFollow.Position;
                transform.Rotation = transToFollow.Rotation;

                transform.LocalPosition -= Vector3.Transform(Vector3.Forward * follow.FollowDistance, transform.LocalRotation);
            }
        }
    }
}
