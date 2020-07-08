using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    [RequiresSystem(typeof(GameTimeSystem))]
    public class GameTimeSingleton : Component
    {
        /// <summary>
        /// This is the real functionality of this component: to store the paused/unpaused state
        /// </summary>
        public bool Paused = false;
        public float UnpauseTimeScale = 1f;

        /// <summary>
        /// Just a wraper around the real TimeScale property
        /// </summary>
        public float TimeScale
        {
            get
            {
                return GameTime.TimeScale;
            }
            set
            {
                GameTime.TimeScale = value;
            }
        }
    }
}
