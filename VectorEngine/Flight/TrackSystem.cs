using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine;

namespace Flight
{
    public class TrackSystem : ECSSystem
    {
        public override void Tick()
        {
            foreach (var track in EntityAdmin.Instance.GetComponents<Track>())
            {
                track.Progress += GameTime.LastFrameTime * track.BaseSpeed;
                track.TrackPoint.LocalPosition = GetTrackPointPosition(track.Progress);
            }
        }

        public static Vector3 GetTrackPointPosition(float value)
        {
            // This could sample a curve or something I guess
            return new Vector3(0, 10, value * -1);
        }
    }
}
