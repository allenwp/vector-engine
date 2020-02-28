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
        //public static 

        public FrameOutput()
        {
            //ASIOOutput.StartDriver();
            // making a different thread with a high priority is pointless because the buffer swap event comes through on a totally different thread
            //Thread thread = new Thread(new ThreadStart(ASIOOutput.ThreadMethod));
            //thread.Name = "ASIOOutput Thread";
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
        }
    }
}
