using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class GameTimeSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var time in EntityAdmin.Instance.GetComponents<GameTimeSingleton>())
            {
                if (time.Paused && time.TimeScale != 0)
                {
                    time.UnpauseTimeScale = time.TimeScale;
                    time.TimeScale = 0;
                }
                else if (!time.Paused && time.TimeScale == 0 && time.UnpauseTimeScale != 0)
                {
                    time.TimeScale = time.UnpauseTimeScale;
                }
            }
        }
    }
}
