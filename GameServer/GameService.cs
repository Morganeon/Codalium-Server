using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using GameServer.NPCs;
using GameServer.AOEs;

namespace GameServer
{
    public class GameService : Service
    {
        List<NPC> npcs;
        List<AOE> aoes;

        public override void Initialize()
        {
            base.Initialize();

            npcs = new List<NPC>();
            NPC npc = new NPC();
            npc.transform.setPosition(new Vector2(5.0f, -5.0f));
            npcs.Add(npc);

            aoes = new List<AOE>();
            AOE aoe = new AOE();
            aoe.transform.setPosition(new Vector2(-5.0f, 5.0f));
            aoes.Add(aoe);

            serviceName = "GAME";
        }

        //
        // On attends que le client nous dise s'il veut faire un donjon, auquel cas on le redirige vers un service approprié
        // 
        public override void ProcessClient(Client c)
        {
            ByteMessage byteMessage = c.GetByteMessage();

            switch (byteMessage.ReadTag())
            {
                
                case "OMV": // Movement intent
                    {
                        int id = byteMessage.ReadInt();
                        int segments = byteMessage.ReadInt();
                        List<float> dx = new List<float>();
                        List<float> dy = new List<float>();
                        List<float> rx = new List<float>();
                        List<float> ry = new List<float>();
                        List<float> time = new List<float>();
                        for (int i=0;i<segments;i++)
                        {
                            dx.Add(byteMessage.ReadFloat());
                            dy.Add(byteMessage.ReadFloat());
                            rx.Add(byteMessage.ReadFloat());
                            ry.Add(byteMessage.ReadFloat());
                            time.Add(byteMessage.ReadFloat());
                        }
                        c.actions[0] = new Actions.MoveAction(dx, dy, rx, ry, time);
                    }
                    break;

                case "PNG":
                    {
                        c.HeartBeat();
                    }
                    break;
                
            }
        }

        public override void Routine(float deltatime)
        {
            
            // Application des intentions
            foreach (Client c in clients)
            {
                for (int i = 0; i < Client.nbActions; i++)
                    if (c.actions[i] != null) c.actions[i].Execute(c);

                c.NullActions();
            }
            // Mise à jour des joueurs
            foreach (Client c in clients)
            {
                c.transform.Update(deltatime);
            }
            // Envoi des mises à jours
            ByteMessage snapshot = new ByteMessage();
            snapshot.WriteTag("SNP");


            // For Each Client
            snapshot.WriteInt(clients.Count);
            foreach (Client c in clients)
            {
                snapshot.WriteInt(c.id);
                snapshot.WriteFloat(c.transform.getPosition().X);
                snapshot.WriteFloat(c.transform.getPosition().Y);
            }

            // For Each Mob
            snapshot.WriteInt(npcs.Count);
            foreach (NPC c in npcs)
            {
                snapshot.WriteInt(c.id);
                snapshot.WriteFloat(c.transform.getPosition().X);
                snapshot.WriteFloat(c.transform.getPosition().Y);
            }

            // For Each AoE
            snapshot.WriteInt(aoes.Count);
            foreach (AOE c in aoes)
            {
                snapshot.WriteInt(c.id);
                snapshot.WriteFloat(c.transform.getPosition().X);
                snapshot.WriteFloat(c.transform.getPosition().Y);
            }

            // Send!

            foreach (Client c in clients)
            {
                c.SendMessage(snapshot);
            }

            // Old update
            /*
            foreach (Client c in clients)
            {
                ByteMessage msg = new ByteMessage();
                msg.WriteTag("UPD");
                msg.WriteInt(c.id);
                msg.WriteFloat(c.transform.getPosition().X);
                msg.WriteFloat(c.transform.getPosition().Y);
                foreach (Client c2 in clients) c2.SendMessage(msg);

            }*/
        }


        public override void OnServiceEnter(Client c)
        {
            ByteMessage msg = new ByteMessage();
            msg.WriteTag("NEW");
            msg.WriteInt(c.id);
            msg.WriteFloat(1.0f);
            msg.WriteFloat(1.0f);
            // Player generation
            foreach ( Client c2 in clients)
            {
                c2.SendMessage(msg);
                // Create every player pawns in the incoming player's context;
                ByteMessage msg2 = new ByteMessage();
                msg2.WriteTag("NEP");
                msg2.WriteInt(c2.id);
                msg2.WriteFloat(c2.transform.getPosition().X);
                msg2.WriteFloat(c2.transform.getPosition().Y);
                c.SendMessage(msg2);
                // Create new player in every other player's context;
                
            }

            // NPCs generation
            foreach (NPC npc in npcs)
            {
                ByteMessage msg2 = new ByteMessage();
                msg2.WriteTag("NEN");
                msg2.WriteInt(npc.id);
                msg2.WriteFloat(npc.transform.getPosition().X);
                msg2.WriteFloat(npc.transform.getPosition().Y);
                c.SendMessage(msg2);

            }

            // AoE generations

            foreach (AOE aoe in aoes)
            {
                ByteMessage msg2 = new ByteMessage();
                msg2.WriteTag("NEA");
                msg2.WriteInt(aoe.id);
                msg2.WriteFloat(aoe.transform.getPosition().X);
                msg2.WriteFloat(aoe.transform.getPosition().Y);
                c.SendMessage(msg2);

            }
        }

        public override void OnServiceLeave(Client c)
        {
            ByteMessage msg = new ByteMessage();
            msg.WriteTag("DEP");
            msg.WriteInt(c.id);

            // Delete client in every player's context
            foreach (Client c2 in clients)
            {
                c2.SendMessage(msg);
            }
        }
    }
}
