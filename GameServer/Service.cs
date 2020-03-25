#define MONITOR_TIME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GameServer
{
    public class Service
    {
        protected List<Client> clients;
        protected List<Client> incomingClients;
        protected List<Client> outgoingClients;

        protected long lastTick;

        private Mutex listProtector;

        protected string serviceName = "DEFAULT";

        protected long loopduration;
        protected int tickNumber = 0;

        public virtual void Run()
        {
            Initialize();
            lastTick = DateTime.Now.Ticks;
#if MONITOR_TIME
            long time;
#endif
            while (true)
            {
                UpdateClients();
#if MONITOR_TIME
                time = DateTime.Now.Ticks;
#endif
                foreach (Client c in clients)
                {
                    if (CheckValidity(c) == false) continue;

                    while (c.HasMessage())
                    {
                        c.HeartBeat();
                        ProcessClient(c);
                    }
                }
                
                while((float)(DateTime.Now.Ticks - lastTick) / (float)TimeSpan.TicksPerMillisecond < 30)
                {

                }
                Routine((float)(DateTime.Now.Ticks - lastTick) / (float)TimeSpan.TicksPerSecond);
                lastTick = DateTime.Now.Ticks;

#if MONITOR_TIME

                loopduration += DateTime.Now.Ticks- time;
                tickNumber++;
                if (tickNumber == 100000)
                {
                    double time_ticks = (loopduration / 100000.0)/ TimeSpan.TicksPerMillisecond;
                    Console.WriteLine("[{0}] Average time in milliseconds per loop: {1}",serviceName, time_ticks);
                    tickNumber = 0;
                    loopduration = 0;
                }
#endif

            }
        }

        public virtual void Routine(float deltatime)
        {

        }

        public virtual void Initialize()
        {
            clients = new List<Client>();
            incomingClients = new List<Client>();
            outgoingClients = new List<Client>();
            listProtector = new Mutex();
        }

        public virtual void ProcessClient(Client c)
        {

        }

        public virtual void OnServiceEnter(Client c)
        {

        }

        public virtual void OnServiceLeave(Client c)
        {

        }

        public void ReceiveClient(Client c)
        {
            listProtector.WaitOne();
            incomingClients.Add(c);
            listProtector.ReleaseMutex();
        }

        public void DropClient(Client c)
        {
            listProtector.WaitOne();
            outgoingClients.Add(c);
            listProtector.ReleaseMutex();
        }

        protected bool CheckValidity(Client c)
        {
            if (DateTime.Now.Ticks - c.LastAlive() > TimeSpan.TicksPerSecond*20) // 20 second timeout
            {
                c.Close();
                DropClient(c);
                return false;
            }

            if (!c.IsConnected()) // express Disconnect
            {
                DropClient(c);
                return false;
            }

            return true;
        }

        protected void UpdateClients()
        {
            listProtector.WaitOne();
            if (outgoingClients.Count != 0)
            {
                clients.RemoveAll(l => outgoingClients.Contains(l));
                foreach (Client c in outgoingClients)
                {
                    OnServiceLeave(c);
                }
                outgoingClients.Clear();
            }

            if (incomingClients.Count != 0)
            {
                foreach (Client c in incomingClients)
                {
                    OnServiceEnter(c);
                }
                clients.AddRange(incomingClients);
                incomingClients.Clear();
            }
            listProtector.ReleaseMutex();
        }
    }
}
