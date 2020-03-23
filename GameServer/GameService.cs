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
                /*
                case "MOV": // Positional update
                    {
                        int id = byteMessage.ReadInt();
                        foreach (Client c2 in clients)
                        {
                            // [TODO] : Broadcasting update as is, might wanna check for valid data.
                            c2.SendMessage(byteMessage);
                        }
                    }
                    break;
                */
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
