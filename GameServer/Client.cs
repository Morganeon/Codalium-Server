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

    public class Client
    {
        private TcpClient client;
        private SslStream stream;
        private long timestamp;

        public int id;
        public long rid=-1;

        public const int nbActions = 2;
        public Actions.Action[] actions;

        public Client(TcpClient client, SslStream stream)
        {
            actions = new Actions.Action[nbActions];
            this.client = client;
            this.stream = stream;
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

        public void NullActions()
        {
            for (int i = 0; i < nbActions; i++) actions[nbActions] = null;
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

    }
}
