using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.EditorHelper;

namespace VectorEngine.Extras
{
    [RequiresSystem(typeof(DisplayProfileAdjusterSystem))]
    public class DisplayProfileAdjuster : Component
    {
        [Newtonsoft.Json.JsonIgnore]
        public bool Initialized = false;
        [Range(0.01f, 2f)]
        public float ApsectRatio;
        [Range(-1f, 1f)]
        public float FullBrightnessOutput;
        [Range(-1f, 1f)]
        public float ZeroBrightnessOutput;
        [Range(0.01f, 5f)]
        public float FidelityScale;
    }
}
