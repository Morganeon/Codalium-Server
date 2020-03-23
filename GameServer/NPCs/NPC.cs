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

        public DialogComponent dialogComponent = null;

        public void Interact(int choice, PlayerToNPCInteractionContext context) // -1: ouverture, autre: choix
        {
            if (dialogComponent == null) return;

            ByteMessage msg = new ByteMessage();
            msg.WriteTag("NPD");
            msg.WriteString("Dialogue de base");
            int nb_options = 2;
            msg.WriteInt(nb_options);

            msg.WriteString("Option 1");
            msg.WriteString("Option 2");


            if (choice == -1)
            {
                context.currentScreen = 0;

            }

            context.parent.SendMessage(msg);
        }

    }
}
