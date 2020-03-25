﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GameServer.NPCs
{
    class NPC
    {
        public int id;
        public Components.Transform transform;

        public NPC()
        {
            Awake();
        }

        public virtual void Awake()
        {
            transform = new Components.Transform();
        }

        public virtual void Update(float deltatime)
        {
            transform.Update(deltatime);
        }

    }
}
