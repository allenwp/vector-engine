using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class GameScene
    {
        public static void Init()
        {
            var cube1 = CreateCube();
            var cube2 = CreateCube();
            cube2.GetComponent<Transform>().Position.X += 2f;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        CreateGridPoint(new Vector3(i * 0.5f, (k + 1) * -0.5f, j * 0.5f));
                    }
                }
            }
        }

        public static Entity CreateCube()
        {
            var entity = new Entity();
            entity.AddComponent<Transform>();
            entity.AddComponent<Cube>();
            return entity;
        }

        public static Entity CreateGridPoint(Vector3 pos)
        {
            var entity = new Entity();
            var trans = entity.AddComponent<Transform>();
            entity.AddComponent<GridPoint>();
            trans.Position = pos;
            return entity;
        }
    }
}
