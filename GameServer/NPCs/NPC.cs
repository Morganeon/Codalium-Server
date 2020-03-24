using System;
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
        public float moveSpeed;
        public Vector3 originalPosition;
        public Vector3 position;
        public List<Vector3> patrolPoints;
        public int currentPatrolPoint;
        public bool isLoop;
        public bool isForward;


    }
}
