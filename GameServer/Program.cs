using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using GameServer;

namespace GameServer
{
    public sealed class SslTcpServer
    {
        
        public static class ServiceBay
        {
            public static LoginService loginService;
            public static UpdateService updateService;
            public static GameService gameService;

            public static void Initialize()
            {
                loginService = new LoginService();
                updateService = new UpdateService();
                gameService = new GameService();
            }
        }


        static List<Client> activeClients;
        static List<Client> pendingClients;
        static List<Client> deleteClients;

        static X509Certificate serverCertificate = null;


        public static void RunServer()
        {
            string certificate = "C:/OpenSsl/mycert.pfx";
            serverCertificate = new X509Certificate2(certificate, "azer");
            TcpListener listener = new TcpListener(IPAddress.Any, 2100);
            listener.Start();
            activeClients = new List<Client>();
            pendingClients = new List<Client>();
            deleteClients = new List<Client>();
            Console.WriteLine("Waiting Clients...");
            while (true)
            {
                // Accept Clients
                TcpClient tcpc = listener.AcceptTcpClient();
                Client c = new Client(tcpc, new SslStream(tcpc.GetStream(), false));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                c.ServerSideSSL(serverCertificate, clientCertificateRequired: false, enabledSslProtocols: SslProtocols.Tls12, checkCertificateRevocation: false);

                ServiceBay.updateService.ReceiveClient(c);
            }
        }
        
        public static int Main(string[] args)
        {

            ServiceBay.Initialize();
            // Map name, Navigation file, Population File
            List<Tuple<string,ByteMessage,ByteMessage>> mapsToBake = new List<Tuple<string, ByteMessage, ByteMessage>>();

            string database = "D:/Database/Maps/";

            string name;
            name = "default";
            mapsToBake.Add(new Tuple<string, ByteMessage, ByteMessage>(name, new ByteMessage(database+name+"-nav"), new ByteMessage(database+name+"-pop")));


            NavigationThread.Populate(mapsToBake);
            ServiceBay.gameService.CreateNPCs(mapsToBake);
            
            Thread Navigator = new Thread(new ThreadStart(NavigationThread.Run));
            Navigator.Start();

            Thread t1 = new Thread(new ThreadStart(SslTcpServer.RunServer));
            t1.Start();
            Thread t2 = new Thread(new ThreadStart(ServiceBay.loginService.Run));
            t2.Start();
            Thread t3 = new Thread(new ThreadStart(ServiceBay.updateService.Run));
            t3.Start();
            Thread t4 = new Thread(new ThreadStart(ServiceBay.gameService.Run));
            t4.Start();

            return 0;
        }
    }
}