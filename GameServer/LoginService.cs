﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class LoginService : Service
    {
        static int id = 0;
        public override void Initialize()
        {
            base.Initialize();

            serviceName = "LOGIN";
        }

        public override void ProcessClient(Client c)
        {
            ByteMessage byteMessage = c.GetByteMessage();

            switch (byteMessage.ReadTag())
            {
                case "CRD":
                    string username = byteMessage.ReadString();
                    string password = byteMessage.ReadString();
                    Console.WriteLine("Incoming credentials: Username {0}, Password {1}, Clients logging in: {2}", username, password, clients.Count);

                    // Successfully received credentials, tell user it's allright, give it an id and transfer it to the game service
                    ByteMessage ok = new ByteMessage();
                    ok.WriteTag("LOK");
                    ok.WriteInt(id);
                    c.id = id;
                    id++;
                    c.SendMessage(ok);
                    //c.Load();
                    SslTcpServer.ServiceBay.gameService.ReceiveClient(c);
                    DropClient(c);
                    break;
            }
        }
    }
}