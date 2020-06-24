using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.DemoGame.DemoGame.Shapes
{
    public class DemoShape : Shape
    {
        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            List<Sample3D[]> result = new List<Sample3D[]>(1);

            //Sample3D[] sample3DArray = new Sample3D[1];
            //sample3DArray[0].Position = new Vector3(0, 0, 0);
            //sample3DArray[0].Brightness = 1f;



            //int sampleLength = 30;
            //Sample3D[] sample3DArray = new Sample3D[sampleLength];
            //for (int i = 0; i < sampleLength; i++)
            //{
            //    sample3DArray[i].Position = new Vector3(0, 0, 0);
            //    sample3DArray[i].Brightness = 1f;
            //}



            //Sample3D[] sample3DArray = new Sample3D[4];
            //sample3DArray[0].Position = new Vector3(-0.5f, 0, 0);
            //sample3DArray[0].Brightness = 1f;

            //sample3DArray[1].Position = new Vector3(-0.25f, 0f, 0);
            //sample3DArray[1].Brightness = 1f;

            //sample3DArray[2].Position = new Vector3(0.25f, -0f, 0);
            //sample3DArray[2].Brightness = 1f;

            //sample3DArray[3].Position = new Vector3(0.5f, 0, 0);
            //sample3DArray[3].Brightness = 1f;



            int sampleLength = 300;
            Sample3D[] sample3DArray = new Sample3D[sampleLength];
            Vector3 Start = new Vector3(-0.75f, 0, 0);
            Vector3 End = new Vector3(0.75f, 0, 0);
            for (int i = 0; i < sampleLength; i++)
            {
                var point3D = Vector3.Lerp(Start, End, (float)i / (float)(sampleLength - 1));
                sample3DArray[i].Position = point3D;
                sample3DArray[i].Brightness = 1f;
            }

            result.Add(sample3DArray);

            return result;
        }
    }
}
