using Flight.Shapes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Extras.PostProcessing;
using VectorEngine.PostProcessing;

namespace Flight
{

    public class FieldSystem : ECSSystem
    {
        static Random rand = new Random();

        public override void Tick()
        {
            foreach (var field in EntityAdmin.Instance.GetComponents<Field>())
            {
                var cam = EntityAdmin.Instance.GetComponents<Camera>().First();
                var camTrans = cam.Entity.GetComponent<Transform>();
                foreach (var obj in field.Objects)
                {
                    if (obj.Position.Z > camTrans.Position.Z)
                    {
                        if (obj.Entity.GetComponent<Dot>() != null)
                        {
                            obj.LocalPositionZ -= field.Depth;
                        }
                        else
                        {
                            obj.LocalPosition = camTrans.Position;
                            obj.LocalPosition.Z -= field.Depth;
                            obj.LocalPosition.Y = 0; // not affected by camera;
                            obj.LocalPosition.X = (float)rand.NextDouble() * field.Width - (field.Width / 2f);
                        }
                    }
                }
            }
        }

        public static void PopulateField(Field field)
        {
            field.Objects = new List<Transform>();
            for (int i = 0; i < field.SpireObjectCount; i++)
            {
                var position = new Vector3();
                position.Z = ((float)i / field.SpireObjectCount) * field.Depth * -1f;
                position.Y = 0;
                position.X = (float)rand.NextDouble() * field.Width - (field.Width / 2f);
                field.Objects.Add(CreateCurlySpire(position));
            }

            bool countUp = true;
            for (int ix = 0; ix < field.DotCountX; ix++)
            {
                float xPos = ((float)ix / (field.DotCountX - 1)) * field.Width - (field.Width / 2f);
                if (countUp)
                {
                    for (int iz = 0; iz < field.DotCountZ; iz++)
                    {
                        float zPos = ((float)iz / field.DotCountZ - 1) * field.Depth * -1f;
                        Vector3 dotPos = new Vector3(xPos, 0, zPos);
                        field.Objects.Add(CreateDot(dotPos));
                    }    
                }
                else
                {
                    for (int iz = field.DotCountZ - 1; iz >= 0; iz--)
                    {
                        float zPos = ((float)iz / field.DotCountZ - 1) * field.Depth * -1f;
                        Vector3 dotPos = new Vector3(xPos, 0, zPos);
                        field.Objects.Add(CreateDot(dotPos));
                    }
                }
            }
        }

        private static Transform CreateCurlySpire(Vector3 position)
        {
            var entity = EntityAdmin.Instance.CreateEntity("CurlySpire");
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            trans.LocalPosition = position;
            trans.LocalScale.Y = 20f;
            trans.LocalScale.Z = trans.LocalScale.X = 8f;
            EntityAdmin.Instance.AddComponent<CurlySpire>(entity);
            //EntityAdmin.Instance.AddComponent<Rotate>(entity); // meh, I could go either way
            var strobe = EntityAdmin.Instance.AddComponent<StrobePostProcessor>(entity);
            strobe.AnimationSpeed = 1.7f;
            strobe.Scale = -33f;
            strobe.AnimationValue = (float)rand.NextDouble();
            var ppg = EntityAdmin.Instance.AddComponent<PostProcessingGroup3D>(entity);
            ppg.PostProcessors.Add(strobe);
            return trans;
        }

        private static Transform CreateDot(Vector3 position)
        {
            var entity = EntityAdmin.Instance.CreateEntity("Dot");
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            trans.LocalPosition = position;
            EntityAdmin.Instance.AddComponent<Dot>(entity);
            return trans;
        }
    }
}
