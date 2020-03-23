using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class UpdateService : Service
    {
        public override void Initialize()
        {
            base.Initialize();

            serviceName = "UPDATE";
        }

        public override void ProcessClient(Client c)
        {
            ByteMessage byteMessage = c.GetByteMessage();

            switch (byteMessage.ReadTag())
            {
                case "VER":
                    string version = byteMessage.ReadString();
                    Console.WriteLine("Client Version: {0}", version);

                    SslTcpServer.ServiceBay.loginService.ReceiveClient(c);

                    //Sends user to the login service and ask for credentials (AFC)
                    ByteMessage creds = new ByteMessage();
                    creds.WriteTag("AFC");
                    c.SendMessage(creds);

                    DropClient(c);
                    break;
            }
        }
    }
}
