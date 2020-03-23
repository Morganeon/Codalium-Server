using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{

    class NavigationThread
    {
        static List<Tuple<string, Vector3, Vector3,long>> requests;
        static List<Tuple<long, List<Vector3>>> results;

        static long requestId;

        static Mutex requestMutex;
        static Mutex resultsMutex;

        public static void Populate( List<Tuple<string,ByteMessage,ByteMessage>> mapsToBake)
        {
            NavigationBuilder.Initialize();
            foreach (var mapdata in mapsToBake)
            {
                ByteMessage map = mapdata.Item2;
                //map.Load("D:/Database/Maps/default");
                string name = mapdata.Item1;
                int nbVert = map.ReadInt();

                Vector3[] verts = new Vector3[nbVert];

                for (int i = 0; i < nbVert; i++)
                {
                    verts[i] = new Vector3(map.ReadFloat(), map.ReadFloat(), map.ReadFloat());
                }


                int nbInd = map.ReadInt();
                int[] inds = new int[nbInd];

                for (int i = 0; i < nbInd; i++)
                {
                    inds[i] = map.ReadInt();
                }

                int nbPg = map.ReadInt();
                List<NavigationBuilder.PointGraph> pts = new List<NavigationBuilder.PointGraph>();

                for (int i = 0; i < nbPg; i++)
                {
                    NavigationBuilder.PointGraph p = new NavigationBuilder.PointGraph();
                    p.index = map.ReadInt();
                    p.neighbours = new List<int>();

                    int nbNei = map.ReadInt();

                    for (int j = 0; j < nbNei; j++)
                    {
                        p.neighbours.Add(map.ReadInt());
                    }

                    pts.Add(p);
                }





                NavigationBuilder.AddMap(name, verts, inds, pts);
            }

           

        }

        public static long AddRequest(string name, Vector3 from, Vector3 to)
        {
            long id;
            requestMutex.WaitOne();

            id = requestId;
            requestId++;

            requests.Add(new Tuple<string, Vector3, Vector3, long>(name,from,to,id));

            requestMutex.ReleaseMutex();

            return id;

        }

        public static List<Vector3> TryGetResult(long requestId)
        {
            List<Vector3> list = null;
            resultsMutex.WaitOne();
            foreach(var res in results)
            {
                if (res.Item1 == requestId)
                {
                    list = res.Item2;
                    results.Remove(res);
                    break;
                }
            }

            if (requests.Count == 0 && results.Count == 0) requestId = 0; // DIRTY FIX TO REQUEST OVERFLOW, maybe doing request double buffering will fix that if the problew ever shows

            resultsMutex.ReleaseMutex();

            return list;
        }

        public static void Run()
        {
            requests = new List<Tuple<string, Vector3, Vector3, long>>();
            results = new List<Tuple<long, List<Vector3>>>();
            requestMutex = new Mutex();
            resultsMutex = new Mutex();
            while (true)
            {
                if (requests.Count !=0)
                {
                    requestMutex.WaitOne();

                    var data = requests.First();
                    requests.Remove(data);

                    

                    requestMutex.ReleaseMutex();


                    var list = NavigationBuilder.GetShortestPath(data.Item1, data.Item2, data.Item3);


                    resultsMutex.WaitOne();

                    results.Add(new Tuple<long,List<Vector3>>(data.Item4,list));

                    resultsMutex.ReleaseMutex();
                }
            }
        }
    }
}
