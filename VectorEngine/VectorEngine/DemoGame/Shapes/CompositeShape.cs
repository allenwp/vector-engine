using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame.Shapes
{
    public class CompositeShape : Shape
    {
        public enum OperationEnum { Add, Subtract, Multiply, Divide }
        public delegate Sample3D[] ShapeDelegate(int sampleCount);

        public ShapeDelegate BaseShape;
        public ShapeDelegate SecondShape;

        public OperationEnum Operation = OperationEnum.Add;
        public float SecondSampleLengthScale = 1f;
        public Vector3 SecondSampleScale = Vector3.One;

        public override List<Sample3D[]> GetSamples3D(float fidelity)
        {
            int baseSampleCount = 1000;
            var result = new List<Sample3D[]>(1);
            int sampleCount = (int)Math.Round(baseSampleCount * fidelity);
            var baseSamples = BaseShape(sampleCount);
            var secondSamples = SecondShape((int)Math.Round(sampleCount * SecondSampleLengthScale));

            int j = 0;
            for (int i = 0; i < baseSamples.Length; i++)
            {
                var secondSample = secondSamples[j].Position;
                secondSample *= SecondSampleScale;

                switch (Operation)
                {
                    case OperationEnum.Add:
                        baseSamples[i].Position += secondSample;
                        break;
                    case OperationEnum.Subtract:
                        baseSamples[i].Position -= secondSample;
                        break;
                    case OperationEnum.Multiply:
                        baseSamples[i].Position *= secondSample;
                        break;
                    case OperationEnum.Divide:
                        baseSamples[i].Position /= secondSample;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                j++;
                if (j >= secondSamples.Length)
                {
                    j = 0;
                }
            }

            result.Add(baseSamples);

            return result;
        }
    }
}
