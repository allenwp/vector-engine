using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class CurlyCircleSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var curlyCircle in EntityAdmin.Instance.GetComponents<CurlyCircle>())
            {
                curlyCircle.AnimationOffset += curlyCircle.AnimationSpeed * GameTime.LastFrameTime;
            }
        }
    }
}
