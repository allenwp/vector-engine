using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VectorEngine.Output
{
    public class FrameOutput
    {
        public FrameOutput()
        {
            ASIOOutput output = new ASIOOutput();
            Thread thread = new Thread(new ThreadStart(ASIOOutput.ThreadMethod));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
