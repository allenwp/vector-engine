using Flight.Shapes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;
using VectorEngine.Extras;
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
                            obj.LocalPosition.Z -= field.Depth;
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

        public static void PopulateField(Field field, Transform player)
        {
            field.Objects = new List<Transform>();

            // Spires
            for (int i = 0; i < field.SpireObjectCount; i++)
            {
                var position = new Vector3();
                position.Z = ((float)i / field.SpireObjectCount) * field.Depth * -1f;
                position.Y = 0;
                position.X = (float)rand.NextDouble() * field.Width - (field.Width / 2f);
                field.Objects.Add(CreateCurlySpire(position, player));
            }

            // Spinning cubes
            for (int i = 0; i < field.SpinningCubeObjectCount; i++)
            {
                var position = new Vector3();
                float distanceBetween = field.Depth / field.SpinningCubeObjectCount;
                position.Z = ((float)i / field.SpinningCubeObjectCount) * field.Depth * -1f + (distanceBetween / 2f);
                position.Y = (float)rand.NextDouble() * 10f + 10f;
                position.X = (float)rand.NextDouble() * 30f - 15f; // 30 is range player can move
                field.Objects.Add(CreateSpinnyCube(position));
            }

            // Floor grid
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

        private static Transform CreateCurlySpire(Vector3 position, Transform player)
        {
            float width = 8f;
            var entity = EntityAdmin.Instance.CreateEntity("CurlySpire");
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            trans.LocalPosition = position;
            trans.LocalScale.Y = 20f;
            trans.LocalScale.Z = trans.LocalScale.X = width;
            EntityAdmin.Instance.AddComponent<CurlySpire>(entity);
            EntityAdmin.Instance.AddComponent<Rotate>(entity).Speed = 2.5f;
            var strobe = EntityAdmin.Instance.AddComponent<StrobePostProcessor>(entity);
            strobe.AnimationSpeed = 1.7f;
            strobe.Scale = -33f;
            strobe.AnimationValue = (float)rand.NextDouble();
            strobe.SelfEnabled = false;
            var ppg = EntityAdmin.Instance.AddComponent<PostProcessingGroup3D>(entity);
            ppg.PostProcessors.Add(strobe);

            var collision = EntityAdmin.Instance.AddComponent<StaticBurstCollision>(entity);
            collision.Player = player;
            collision.DistanceFromPlayer = 10;//width;// / 2f;

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

        private static Transform CreateSpinnyCube(Vector3 position)
        {
            int dotSampleCount = 100;

            var entity = EntityAdmin.Instance.CreateEntity("SpinnyCube");
            var trans = EntityAdmin.Instance.AddComponent<Transform>(entity);
            trans.LocalPosition = position;
            trans.LocalScale = new Vector3(10f, 10f, 10f);
            EntityAdmin.Instance.AddComponent<Dot>(entity).SampleCount = dotSampleCount;

            AddCube(trans);

            AddOrbitingDot(trans, new Vector3(0.2f, 0f, 0), RandomQuaternion(), 0.4f, dotSampleCount);
            AddOrbitingDot(trans, new Vector3(0.2f, 0f, 0), RandomQuaternion(), 0.4f, dotSampleCount);
            AddOrbitingDot(trans, new Vector3(0.2f, 0f, 0), RandomQuaternion(), 0.4f, dotSampleCount);
            AddOrbitingDot(trans, new Vector3(0.2f, 0f, 0), RandomQuaternion(), 0.4f, dotSampleCount);

            return trans;
        }

        private static void AddOrbitingDot(Transform rootParent, Vector3 position, Quaternion dotLevel1Rotation, float rotateSpeed, int dotSampleCount)
        {
            var entity = EntityAdmin.Instance.CreateEntity("Dot Level 1");
            var dotLevel1 = EntityAdmin.Instance.AddComponent<Transform>(entity);
            dotLevel1.LocalRotation = dotLevel1Rotation;
            Transform.AssignParent(dotLevel1, rootParent);

            entity = EntityAdmin.Instance.CreateEntity("Dot Level 2");
            var dotLevel2 = EntityAdmin.Instance.AddComponent<Transform>(entity);
            EntityAdmin.Instance.AddComponent<Rotate>(entity).Speed = rotateSpeed;
            Transform.AssignParent(dotLevel2, dotLevel1);

            entity = EntityAdmin.Instance.CreateEntity("Dot Level 3 (Shape)");
            var dotLevel3 = EntityAdmin.Instance.AddComponent<Transform>(entity);
            dotLevel3.LocalPosition = position;
            EntityAdmin.Instance.AddComponent<Dot>(entity).SampleCount = dotSampleCount;
            Transform.AssignParent(dotLevel3, dotLevel2);
        }

        private static void AddCube(Transform rootParent)
        {
            float maxSpeed = 1f;

            var entity = EntityAdmin.Instance.CreateEntity("Cube Level 1");
            var cubeLevel1 = EntityAdmin.Instance.AddComponent<Transform>(entity);
            var rotate = EntityAdmin.Instance.AddComponent<Rotate>(entity);
            rotate.Speed = ((float)rand.NextDouble() - 0.5f) * maxSpeed;
            rotate.Axis = Rotate.AxisEnum.x;
            Transform.AssignParent(cubeLevel1, rootParent);

            entity = EntityAdmin.Instance.CreateEntity("Cube Level 2");
            var cubeLevel2 = EntityAdmin.Instance.AddComponent<Transform>(entity);
            rotate = EntityAdmin.Instance.AddComponent<Rotate>(entity);
            rotate.Speed = ((float)rand.NextDouble() - 0.5f) * maxSpeed;
            rotate.Axis = Rotate.AxisEnum.y;
            Transform.AssignParent(cubeLevel2, cubeLevel1);

            entity = EntityAdmin.Instance.CreateEntity("Cube Level 3 (Shape)");
            var cubeLevel3 = EntityAdmin.Instance.AddComponent<Transform>(entity);
            cubeLevel3.LocalScale = new Vector3(0.4f, 0.4f, 0.4f);
            rotate = EntityAdmin.Instance.AddComponent<Rotate>(entity);
            rotate.Speed = ((float)rand.NextDouble() - 0.5f) * maxSpeed;
            rotate.Axis = Rotate.AxisEnum.z;
            EntityAdmin.Instance.AddComponent<Cube>(entity);
            Transform.AssignParent(cubeLevel3, cubeLevel2);
        }

        private static Quaternion RandomQuaternion()
        {
            return Quaternion.CreateFromYawPitchRoll((float)rand.NextDouble() * MathHelper.TwoPi, (float)rand.NextDouble() * MathHelper.TwoPi, (float)rand.NextDouble() * MathHelper.TwoPi);
        }
    }
}
