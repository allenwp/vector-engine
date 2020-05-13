using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    public class ShadowSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var (transform, shadow) in EntityAdmin.Instance.GetTuple<Transform, Shadow>())
            {
                transform.LocalPosition = shadow.ShadowSource.Position;
                transform.LocalPosition.Y = 0;
            }
        }
    }
}
