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
        float tickDelta = 0;
        float tickTimer = 1.0f/128.0f;
        public override void Initialize()
        {
            base.Initialize();

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
            foreach (Client c in clients)
            {
                ByteMessage msg = new ByteMessage();
                msg.WriteTag("UPD");
                msg.WriteInt(c.id);
                msg.WriteFloat(c.transform.getPosition().X);
                msg.WriteFloat(c.transform.getPosition().Y);
                foreach (Client c2 in clients) c2.SendMessage(msg);

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
