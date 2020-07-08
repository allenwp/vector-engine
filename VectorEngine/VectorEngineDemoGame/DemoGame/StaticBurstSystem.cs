using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.DemoGame.PostProcessing;

namespace VectorEngine.DemoGame
{
    public class StaticBurstSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var (staticBurst, staticPP) in EntityAdmin.Instance.GetTuple<StaticBurst, StaticPostProcessor>())
            {
                if (staticPP.Amount > 0)
                {
                    staticPP.Amount -= staticBurst.DecaySpeed * GameTime.LastFrameTime;
                    if (staticPP.Amount < 0)
                    {
                        staticPP.Amount = 0;
                    }
                }
            }
        }
    }
}
