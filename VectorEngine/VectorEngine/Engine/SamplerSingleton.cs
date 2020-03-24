using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    // "Singleton Component". See 12:00 mark of https://www.gdcvault.com/play/1024001/-Overwatch-Gameplay-Architecture-and
    // A "Singleton Component" is a component which only has one instance per EntityAdmin and is associated with an annonomous entity.
    public class SamplerSingleton
    {
        public List<Sample[]> LastSamples;
    }
}
