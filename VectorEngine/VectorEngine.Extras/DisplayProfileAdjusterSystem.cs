using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Output;

namespace VectorEngine.Extras
{
    public class DisplayProfileAdjusterSystem : ECSSystem
    {
        public override void Tick(EntityAdmin admin)
        {
            foreach (DisplayProfileAdjuster adjuster in admin.GetComponents<DisplayProfileAdjuster>())
            {
                if (adjuster.Initialized)
                {
                    if (adjuster.FullBrightnessOutput < adjuster.ZeroBrightnessOutput)
                    {
                        adjuster.FullBrightnessOutput = adjuster.ZeroBrightnessOutput;
                    }
                    FrameOutput.DisplayProfile.AspectRatio = adjuster.ApsectRatio;
                    FrameOutput.DisplayProfile.FullBrightnessOutput = adjuster.FullBrightnessOutput;
                    FrameOutput.DisplayProfile.ZeroBrightnessOutput = adjuster.ZeroBrightnessOutput;
                }
                else
                {
                    adjuster.ApsectRatio = FrameOutput.DisplayProfile.AspectRatio;
                    adjuster.FullBrightnessOutput = FrameOutput.DisplayProfile.FullBrightnessOutput;
                    adjuster.ZeroBrightnessOutput = FrameOutput.DisplayProfile.ZeroBrightnessOutput;
                    adjuster.Initialized = true;
                }
            }
        }
    }
}
