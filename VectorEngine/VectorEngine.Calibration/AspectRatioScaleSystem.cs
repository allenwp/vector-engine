using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Calibration
{
    public class AspectRatioScaleSystem : ECSSystem
    {
        public override void Tick(EntityAdmin admin)
        {
            foreach ((Transform transform, AspectRatioScale aspectRatioScale) in admin.GetTuple<Transform, AspectRatioScale>())
            {
                transform.LocalScale.X = FrameOutput.DisplayProfile.AspectRatio;
                transform.LocalScale.Y = 1;
                transform.LocalScale.Z = 1;
            }
        }
    }
}
