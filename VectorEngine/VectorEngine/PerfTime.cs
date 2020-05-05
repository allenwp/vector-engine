using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine
{
    public struct PerfTime
    {
        public float best;
        public float worst;
        public int count;
        public float cumulative;
        public float average { get => cumulative / count; }

        public static readonly PerfTime Initial = new PerfTime(float.MaxValue, float.MinValue, 0, 0);

        public PerfTime(float best, float worst, int count, float cumulative)
        {
            this.best = best;
            this.worst = worst;
            this.count = count;
            this.cumulative = cumulative;
        }

        public static void RecordPerfTime(Stopwatch stopwatch, ref PerfTime perfTime)
        {
            var time = stopwatch.ElapsedMilliseconds;
            perfTime.cumulative += time;
            perfTime.count++;
            if (time > perfTime.worst)
            {
                perfTime.worst = time;
            }
            if (time < perfTime.best)
            {
                perfTime.best = time;
            }
        }
    }
}
