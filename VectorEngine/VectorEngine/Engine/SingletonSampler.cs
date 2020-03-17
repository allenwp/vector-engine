using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    // "Singleton Component". See 12:00 mark of https://www.gdcvault.com/play/1024001/-Overwatch-Gameplay-Architecture-and
    // A "Singleton Component" is a component which only has one instance per EntityAdmin and is associated with an annonomous entity.
    public class SingletonSampler
    {
        // I'm trying to learn how to work with an Entity Component System.
        // I need to store the result of the SamplerSystem somehwere, but the system shouldn't store
        // state like that. So this is where I'm storing the result for now... This is weird, but whatever.
        public List<Sample[]> LastSamples;
    }
}
