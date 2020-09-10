using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Calibration
{
    public class CalibrationLineControllerSystem : ECSSystem
    {
        public override void Tick(EntityAdmin admin)
        {
            foreach (CalibrationLineController controller in admin.GetComponents<CalibrationLineController>())
            {
                foreach (Shapes.CalibrationLine line in controller.Lines.Where(line => line != null))
                {
                    line.BlankingPadding = controller.BlankingPadding;
                    line.RepeatCount = controller.RepeatCount;
                    line.StepDistance = controller.StepDistance;
                }
            }
        }
    }
}
