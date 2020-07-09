using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine.Extras;
using VectorEngine.Extras.PostProcessing;

namespace VectorEngine.DemoGame.DemoGame.MIDIDemo
{
    public class SpireControlSystem : ECSSystem
    {
        public override void Tick()
        {
            var control = EntityAdmin.Instance.GetComponents<SpireControlSingleton>().First();
            if (control != null)
            {
                foreach ((var transform, var spire, var rotate, var strobe) in EntityAdmin.Instance.GetTuple<Transform, CurlySpire, Rotate, StrobePostProcessor>(true))
                {
                    transform.LocalScale = control.Scale;
                    rotate.Speed = control.RotateSpeed;
                    spire.CurlCount = control.CurlCount;
                    strobe.AnimationSpeed = control.StrobeSpeed;
                    strobe.Scale = control.StrobeScale;
                    strobe.SelfEnabled = control.StrobeEnabled;
                }
            }
        }
    }
}
