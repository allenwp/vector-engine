using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine
{
    public struct Sample3DStream
    {
        public int StartIndex;
        public int Length;

        public Sample3D this[int index]
        {
            get => Sample3DPool.Pool[StartIndex + index];
            set => Sample3DPool.Pool[StartIndex + index] = value;
        }

        public Sample3D[] Pool
        {
            get => Sample3DPool.Pool;
        }

        public int PoolIndex(int thisIndex)
        {
            return StartIndex + thisIndex;
        }
    }
}
