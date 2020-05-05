using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public class Tween
    {
        private const float Pi = (float)Math.PI;
        private const float HalfPi = Pi / 2f;

        public static float EaseInPower(float progress, int power)
        {
            return (float)Math.Pow(progress, power);
        }

        public static float EaseOutPower(float progress, int power)
        {
            int sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(progress - 1, power) + sign));
        }

        public static float EaseInOutPower(float progress, int power)
        {
            progress *= 2;
            if (progress < 1)
            {
                return (float)Math.Pow(progress, power) / 2f;
            }
            else
            {
                int sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(progress - 2, power) + sign * 2));
            }
        }

        public static float SineEaseInImpl(float progress)
        {
            return (float)Math.Sin(progress * HalfPi - HalfPi) + 1;
        }

        public static float SineEaseOutImpl(float progress)
        {
            return (float)Math.Sin(progress * HalfPi);
        }

        public static float SineEaseInOutImpl(float progress)
        {
            return (float)(Math.Sin(progress * Pi - HalfPi) + 1) / 2;
        }
    }
}
