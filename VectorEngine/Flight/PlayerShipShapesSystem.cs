using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    public class PlayerShipShapesSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var shapes in EntityAdmin.Instance.GetComponents<PlayerShipShapes>())
            {
                shapes.AnimationState += shapes.AnimationSpeed * GameTime.LastFrameTime;
                while (shapes.AnimationState > 1f)
                {
                    shapes.AnimationState -= 1f;
                }

                for (int i = 0; i < shapes.RingShapes.Count; i++)
                {
                    var ring = shapes.RingShapes[i];
                    var value = shapes.AnimationState + (float)i / shapes.RingShapes.Count;
                    while (value > 1f)
                    {
                        value -= 1f;
                    }

                    ring.LocalPosition.Z = value * shapes.ShipLength - shapes.ShipLength;

                    if (value < shapes.NoseLength)
                    {
                        value = Tween.EaseInPower(value / shapes.NoseLength, 2);
                        ring.LocalScale.X = MathHelper.Lerp(shapes.NoseTipShape.X, shapes.BodyShape.X, value);
                        ring.LocalScale.Y = MathHelper.Lerp(shapes.NoseTipShape.Y, shapes.BodyShape.Y, value);
                        ring.LocalScale.Z = MathHelper.Lerp(shapes.NoseTipShape.Z, shapes.BodyShape.Z, value);
                    }
                    else if (value < shapes.NoseLength + shapes.BodyLength)
                    {
                        ring.LocalScale = shapes.BodyShape;
                    }
                    else
                    {
                        value = (value - shapes.NoseLength - shapes.BodyLength) / (1f - shapes.NoseLength - shapes.BodyLength);
                        value = Tween.EaseOutPower(value, 2);
                        ring.LocalScale.X = MathHelper.Lerp(shapes.BodyShape.X, shapes.WingEndShape.X, value);
                        ring.LocalScale.Y = MathHelper.Lerp(shapes.BodyShape.Y, shapes.WingEndShape.Y, value);
                        ring.LocalScale.Z = MathHelper.Lerp(shapes.BodyShape.Z, shapes.WingEndShape.Z, value);
                    }
                }
            }
        }
    }
}
