using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    [RequiresSystem(typeof(ShadowSystem))]
    public class Shadow : Component
    {
        /// <summary>
        /// Must be a sibling to the shadow
        /// </summary>
        public Transform ShadowSource;
    }
}
