using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.EditorHelper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RangeAttribute : Attribute
    {
        public enum RangeTypeEnum { Float, Int }

        public RangeTypeEnum RangeType;

        public float MinFloat;
        public float MaxFloat;

        public int MinInt;
        public int MaxInt;

        public RangeAttribute(float min, float max)
        {
            RangeType = RangeTypeEnum.Float;
            MinFloat = min;
            MaxFloat = max;
        }

        public RangeAttribute(int min, int max)
        {
            RangeType = RangeTypeEnum.Float;
            MinInt = min;
            MaxInt = max;
        }
    }
}
