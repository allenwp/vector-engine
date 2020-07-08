using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine.Extras;

namespace VectorEngine.DemoGame.DemoGame.MIDIDemo
{
    public class DotsDisk
    {
        public static Transform CreateDisk(int numDots, float initialSize)
        {
            var rootEntity = EntityAdmin.Instance.CreateEntity("Dots Disk Root");
            var transform = EntityAdmin.Instance.AddComponent<Transform>(rootEntity);
            transform.LocalScale = new Vector3(initialSize);
            EntityAdmin.Instance.AddComponent<Rotate>(rootEntity);

            var rand = new Random();
            for (int i = 0; i < numDots; i++)
            {
                var entity = EntityAdmin.Instance.CreateEntity("Dots Disk Dot");
                EntityAdmin.Instance.AddComponent<Dot>(entity);
                var dotTrans = EntityAdmin.Instance.AddComponent<Transform>(entity);
                Vector3 pos = Vector3.Forward;
                pos.Z *= (float)rand.NextDouble();
                pos = Vector3.Transform(pos, Matrix.CreateRotationY((float)(rand.NextDouble() * Math.PI * 2)));
                dotTrans.LocalPosition = pos;
                Transform.AssignParent(dotTrans, transform);
            }

            return transform;
        }
    }
}
