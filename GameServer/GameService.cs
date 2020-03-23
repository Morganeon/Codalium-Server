using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using GameServer.NPCs;

namespace GameServer
{
    public class GameService : Service
    {

        Dictionary<string, List<NPC>> npcList;
        double ticktoRefresh;

        public override void Initialize()
        {
            base.Initialize();

            serviceName = "GAME";
        }

        public override void ProcessClient(Client c)
        {
            ByteMessage byteMessage = c.GetByteMessage();

            switch (byteMessage.ReadTag())
            {
                case "UPD": // Positional update
                    {
                        int id = byteMessage.ReadInt();
                        foreach(Client c2 in clients)
                        {
                            // [TODO] : Might wanna drop the condition, and send the message regardless of wether we send it to the emitter.
                            // [TODO] : Broadcasting update as is, might wanna check for valid data.
                            if (c2.id != id) c2.SendMessage(byteMessage); 
                        }
                    }
                    break;
                case "RQP": // Request pathfinding on map arg0
                    {
                        // [TODO] : Only allows one request per client, may be insufficient later.
                        long rid = NavigationThread.AddRequest("default", new System.Numerics.Vector3(byteMessage.ReadFloat(), byteMessage.ReadFloat(), byteMessage.ReadFloat()), new System.Numerics.Vector3(byteMessage.ReadFloat(), byteMessage.ReadFloat(), byteMessage.ReadFloat()));
                        c.rid = rid;
                    }
                    break;

                case "NIN": // Npc interaction
                    {
                        int id = byteMessage.ReadInt();
                        if (true) // Condition of proximity
                        {
                            npcList["default"][id].Interact(byteMessage.ReadInt(), c.context);
                        }
                    }
                    break;

            }
        }

        public void CreateNPCs(List<Tuple<string, ByteMessage, ByteMessage>> mapsToBake)
        {
            npcList = new Dictionary<string, List<NPC>>();

            foreach (var data in mapsToBake)
            {
                List<NPC> npcs = new List<NPC>();

                int numberOfNpcs = data.Item3.ReadInt();

                for (int i=0;i<numberOfNpcs;i++)
                {
                    NPC obj = new NPC();
                    obj.id = i;
                    obj.isForward = true;
                    obj.currentPatrolPoint = 0;
                    obj.originalPosition = new Vector3(data.Item3.ReadFloat(), data.Item3.ReadFloat(), data.Item3.ReadFloat());
                    NavigationBuilder.NearestTriangle(data.Item1, obj.originalPosition, out obj.originalPosition);
                    obj.position = obj.originalPosition;
                    
                    obj.moveSpeed = data.Item3.ReadFloat();
                    obj.isLoop = data.Item3.ReadBool();
                    int nbPatpoint = data.Item3.ReadInt();
                    obj.patrolPoints = new List<Vector3>();
                    for (int j=0;j<nbPatpoint;j++)
                    {
                        var point = new Vector3(data.Item3.ReadFloat(), data.Item3.ReadFloat(), data.Item3.ReadFloat());
                        NavigationBuilder.NearestTriangle(data.Item1, point, out point);
                        obj.patrolPoints.Add(point);
                    }

                    bool hasDialogComponent = data.Item3.ReadBool();

                    if (hasDialogComponent)
                    {
                        obj.dialogComponent = new DialogComponent();
                    }

                    npcs.Add(obj);
                }


                npcList.Add(data.Item1, npcs);
            }

        }

        private void HandleClient(Client c, float deltatime)
        {
            if (c.rid != -1)
            {
                var result = NavigationThread.TryGetResult(c.rid);
                if (result != null)
                {
                    ByteMessage msg = new ByteMessage();
                    msg.WriteTag("RSP");
                    msg.WriteInt(result.Count);

                    for (int i = 0; i < result.Count; i++)
                    {
                        msg.WriteFloat(result[i].X);
                        msg.WriteFloat(result[i].Y);
                        msg.WriteFloat(result[i].Z);
                    }

                    c.SendMessage(msg);
                    c.rid = -1;

                }
            }
        }

        private void MoveNPC(NPC npc, float deltatime)
        {
            float distanceToTravel = npc.moveSpeed * deltatime;
            Vector3 objective;

            int nextpp = npc.currentPatrolPoint + (npc.isForward ? 1 : -1);

            // Si en fin de parcour
            if (nextpp == npc.patrolPoints.Count)
            {
                // on va à l'origine si on boucle
                if (npc.isLoop)
                {
                    objective = npc.originalPosition;
                    nextpp = 0;
                }
                else // on change de direction si on boucle pas
                {
                    npc.isForward = false;
                    nextpp = nextpp - 2;
                    objective = npc.patrolPoints[nextpp];
                }
            }
            else if (nextpp < 0)
            {
                npc.isForward = true;
                nextpp = nextpp + 2;
                objective = npc.patrolPoints[nextpp];
            }
            else
            {
                objective = npc.patrolPoints[nextpp];
            }
            float distanceToCheckpoint = Vector3.Distance(npc.position, objective);


            if (distanceToTravel > distanceToCheckpoint)
            {

                distanceToTravel = distanceToCheckpoint;
                npc.currentPatrolPoint += (npc.isForward ? 1 : -1);

                if (objective == npc.originalPosition) npc.currentPatrolPoint = -1;

                npc.position = objective;
            }
            else
                npc.position += ((objective - npc.position) / distanceToCheckpoint) * distanceToTravel;
        }

        private void HandleNPC(NPC npc, float deltatime)
        {
            // Patrols the NPC
            if (npc.patrolPoints.Count != 0)
                MoveNPC(npc, deltatime);

            // Dialogs would go there


            // Refresh authoritative position on clients
            if (ticktoRefresh > 0.3)
            {
                ByteMessage msg = new ByteMessage();
                msg.WriteTag("NPC");
                msg.WriteInt(npc.id);
                msg.WriteFloat(npc.position.X);
                msg.WriteFloat(npc.position.Y);
                msg.WriteFloat(npc.position.Z);
                foreach (Client c in clients)
                {
                    c.SendMessage(msg);
                }
            }
        }

        public override void Routine(float deltatime)
        {
            // Allows refreshing routines once in a while.
            ticktoRefresh += deltatime;

            // Handling players
            foreach (Client c in clients)
            {
                HandleClient(c,deltatime);
            }
            
            // Handling NPCs
            /*
            foreach (NPC npc in npcList["default"])
            {
                HandleNPC(npc, deltatime);
            }*/

            // Resets refresh timer if applicable
            if (ticktoRefresh > 0.3)
            {
                ticktoRefresh = 0;
            }
        }

        public override void OnServiceEnter(Client c)
        {
            ByteMessage msg = new ByteMessage();
            msg.WriteTag("NEW");
            msg.WriteInt(c.id);

            foreach( Client c2 in clients)
            {
                // Create every player pawns in the incoming player's context;
                ByteMessage msg2 = new ByteMessage();
                msg2.WriteTag("NEW");
                msg2.WriteInt(c2.id);
                c.SendMessage(msg2);
                // Create new player in every other player's context;
                c2.SendMessage(msg);
            }
        }

        public override void OnServiceLeave(Client c)
        {
            ByteMessage msg = new ByteMessage();
            msg.WriteTag("DEL");
            msg.WriteInt(c.id);

            // Delete client in every player's context
            foreach (Client c2 in clients)
            {
                c2.SendMessage(msg);
            }
        }
    }
}
