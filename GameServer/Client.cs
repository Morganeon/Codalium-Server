using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    // Player to NPC context when initiating dialogs and such
    public class PlayerToNPCInteractionContext
    {
        public int currentScreen;
        public Client parent;
    }


    public class Client
    {
        private TcpClient client;
        private SslStream stream;
        private long timestamp;
        public  PlayerToNPCInteractionContext context;

        public int id;
        public long rid=-1;

        public Client(TcpClient client, SslStream stream)
        {
            this.client = client;
            this.stream = stream;
            context = new PlayerToNPCInteractionContext();
            context.parent = this;
            HeartBeat();
        }

        public void ServerSideSSL(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            stream.AuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation);
        }

        public void ClientSideSSL(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            stream.AuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation);
        }

        public void SendMessage(ByteMessage msg)
        {
            try
            {
                stream.Write(msg.GetMessage());
            }
            catch (Exception e)
            {

            }
                
        }

        public ByteMessage GetByteMessage()
        {
            return new ByteMessage(stream);
        }

        public long LastAlive()
        {
            return timestamp;
        }

        public bool IsConnected()
        {
            return client.Connected;
        }

        public bool HasMessage()
        {
            return client.Available!=0;
        }

        public void HeartBeat()
        {
            timestamp = DateTime.Now.Ticks;
        }

        public void Close()
        {
            stream.Close();
            client.Close();
            //Save();
        }
        /*
        public void ReadMessage( out string tag, out string[] messages)
        {
            byte[] btag = new byte[3];
            stream.Read(btag, 0, 3);
            tag = Encoding.ASCII.GetString(btag);

            byte[] bnbParam = new byte[4];
            stream.Read(bnbParam, 0, 4);
            Int32 nbParam = BitConverter.ToInt32(bnbParam, 0);

            messages = new string[nbParam];

            for (int i = 0; i < nbParam; i++)
            {
                byte[] blen = new byte[4];
                stream.Read(blen, 0, 4);

                Int32 len = BitConverter.ToInt32(blen, 0);
                byte[] bmsg = new byte[len];
                stream.Read(bmsg, 0, len);
                messages[i] = Encoding.ASCII.GetString(bmsg);
            }
        }

        public void Save()
        {

            byte[] bid = BitConverter.GetBytes(id);
            IEnumerable<byte> arrays = bid;

            byte[] len = BitConverter.GetBytes(username.Length);
            byte[] bdata = Encoding.ASCII.GetBytes(username);

            arrays = arrays.Concat(len).Concat(bdata);
            byte[] tstamp = BitConverter.GetBytes(timestamp);

            arrays = arrays.Concat(tstamp);

            File.WriteAllBytes("D:/Database/" + username, arrays.ToArray());
        }

        public void Load()
        {
            if (File.Exists("D:/Database/"+username))
            {
                var bytes = File.ReadAllBytes("D:/Database/" + username);

                id = BitConverter.ToInt64(bytes, 0);

                int len = BitConverter.ToInt32(bytes, 8);
                username = "";
                username = Encoding.ASCII.GetString(bytes, 12, len);

                long lastTime = BitConverter.ToInt64(bytes, 12 + len);

                Console.WriteLine("Read: {0}",username);
            }
        }*/
    }
}
