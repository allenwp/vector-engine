﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorEngine.Engine;

namespace VectorEngine.DemoGame
{
    public class Follow : Component
    {
        public Entity EntityToFollow;
        public float FollowDistance;
    }
}
