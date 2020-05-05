using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.DemoGame.Shapes;
using VectorEngine;

namespace VectorEngine.DemoGame
{
    public class SeaOfWavesSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var sea in EntityAdmin.Instance.GetComponents<SeaOfWaves>())
            {
                sea.AnimationValue += GameTime.LastFrameTime * sea.AnimationSpeed;
                if(sea.AnimationValue > sea.Waves[0].Count)
                {
                    sea.AnimationValue = 0;
                }
                for (int rowIndex = 0; rowIndex < sea.Waves.Count; rowIndex++)
                {
                    var row = sea.Waves[rowIndex];
                    for (int tileIndex = 0; tileIndex < row.Count; tileIndex++)
                    {
                        var tile = row[tileIndex];
                        tile.AnimationValue = sea.AnimationValue - tileIndex;
                    }
                }
            }
        }

        public static List<List<WaveTile>> CreateSea()
        {
            var rowCount = 30;
            var tileCount = 20;

            var result = new List<List<WaveTile>>(rowCount);
            for (int row = 0; row < rowCount; row++)
            {
                var list = new List<WaveTile>(tileCount);
                for (int tileIndex = 0; tileIndex < tileCount; tileIndex++)
                {
                    var entity = new Entity("WaveTile");
                    var tile = entity.AddComponent<WaveTile>();
                    tile.DrawLength = 0.5f;
                    var transform = entity.AddComponent<Transform>();
                    transform.LocalScale = new Vector3(5f, 1f, 1f);
                    transform.LocalPosition = new Vector3(tileIndex * transform.LocalScale.X - ((tileCount * transform.LocalScale.X) / 2f), 0, row * -3f);
                    list.Add(tile);
                }
                result.Add(list);
            }
            return result;
        }
    }
}
