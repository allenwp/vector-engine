using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Calibration
{
    [RequiresSystem(typeof(CalibrationLineControllerSystem))]
    public class CalibrationLineController : Component
    {
        [EditorHelper.Help("Number of blanking samples before and after.")]
        public int BlankingPadding = 50;
        public List<float> BrightnessPattern = new List<float>() { 1f };
        public int RepeatCount = 1;
        public float StepDistance = 0.001f;

        public List<Shapes.CalibrationLine> Lines = new List<Shapes.CalibrationLine>();
    }
}
