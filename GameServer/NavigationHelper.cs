using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Assets.GameServer.GameServer
{
    class NavigationHelper
    {

        public class PointGraph
        {
            public int index;
            public List<int> neighbours;
        }

        public static List<PointGraph> BuildPointGraph(Vector3[] vertices, int[] indices)
        {
            List<PointGraph> list = new List<PointGraph>();
            // Pour chaque vertex, on cherche ses voisins
            for (int i=0;i< vertices.Length;i++)
            {
                PointGraph p = new PointGraph();
                p.neighbours = new List<int>();
                p.index = i;

                // Pour chaque triangles
                for (int j=0;j<indices.Length/3;j++)
                {
                    // si un des indices correspond à self
                    //if (vertices[indices[(j * 3)+0]] == vertices[i] || vertices[indices[(j * 3)+1]] == vertices[i] || vertices[indices[(j * 3)+2]] == vertices[i])
                    if (indices[(j * 3)+0] == i || indices[(j * 3)+1] == i || indices[(j * 3)+2] == i)
                    {
                        // Pour les 3 points du triangle
                        for (int k=0;k<3;k++)
                        {
                            // Si c'est ni self ni déjà présent dans la liste on l'ajoute
                            //if (vertices[indices[(j * 3) + k]] != vertices[i] && !p.neighbours.Contains(indices[(j * 3) + k]))
                            if (indices[(j * 3) + k] != i && !p.neighbours.Contains(indices[(j * 3) + k]))
                            {
                                p.neighbours.Add(indices[(j * 3) + k]);
                            }
                        }
                    }
                }
                //UnityEngine.Debug.Log(p.neighbours.Count);
                list.Add(p);
            }

            return list;
        }

         class Node
        {
            public int index;
            public Node parent;
            public float H;
            public float G;
        }

        public static void ReduceMesh(Vector3[]  oldVertices, int[] oldIndices, out Vector3[] vertices, out int[] indices)
        {
            List<Vector3> verticesList = new List<Vector3>();
            List<int> indicesList = new List<int>();

            // Pour chaque point
            for (int i = 0; i < oldVertices.Length; i++)
            {
                if (!verticesList.Contains(oldVertices[i])) verticesList.Add(oldVertices[i]);
                else continue;


                int id = verticesList.Count - 1;
                // Comparé à chaque autre point
                for (int j=0;j<oldIndices.Length;j++)
                {
                    if (Vector3.Distance(oldVertices[oldIndices[j]],oldVertices[i])<0.00001) oldIndices[j] = id;
                }
                
            }
            vertices = verticesList.ToArray();
            indices = oldIndices;
        }

         static void DebugMesh(List<Node> closedList, Vector3[] oldvertices, out Vector3[] vertices)
        {
            foreach(Node n in closedList)
            {
                oldvertices[n.index].Y = 30;
            }

            vertices = oldvertices;
        }

        public static List<Vector3> GetShortestPath(Vector3[] vertices, int[] indices, Vector3 point1, Vector3 point2)//, out Vector3[] vmesh, out int[] imesh)
        {
            UnityEngine.Debug.Log("Sizeof vert:" + vertices.Length + ", sizeof ind:" + indices.Length);
            
            UnityEngine.Debug.Log("Sizeof vert red:" + vertices.Length + ", sizeof ind red:" + indices.Length);




            //vmesh = vertices;
            //imesh = indices;
            List<Vector3> path = new List<Vector3>();
            // Step 1: Get the two triangles concerned
            Vector3 p1proj; // closest point of p1 in t1
            int t1 = NearestTriangle(vertices, indices, point1, out p1proj);
            Vector3 p2proj; // closest point of p2 in t2
            int t2 = NearestTriangle(vertices, indices, point2, out p2proj);

            //UnityEngine.Debug.Log("ORIGINE:" + vertices[indices[t1 * 3 + 0]] + ", DESTINATION:" + p2proj);

            // if they both reside in the same triangle, the direct path is a line.
            if (t1 == t2)
            {
                path.Add(p1proj);
                path.Add(p2proj);
                //UnityEngine.Debug.Log("Early Exit.");
                return path;
            }

            //UnityEngine.Debug.Log("Va de " + indices[t1 * 3 + 0] + " à " + indices[t2*3+0]);

            // Step 2: A*

            var pointGraphs = BuildPointGraph(vertices, indices);

            Node origin1 = new Node();
            origin1.index = indices[t1 * 3 + 0];
            origin1.parent = null;
            origin1.G = Vector3.Distance(vertices[indices[t1 * 3 + 0]], p1proj);
            origin1.H = origin1.G + Vector3.Distance(vertices[indices[t1 * 3 + 0]], p2proj);

            Node origin2 = new Node();
            origin2.index = indices[t1 * 3 + 1];
            origin2.parent = null;
            origin2.G = Vector3.Distance(vertices[indices[t1 * 3 + 1]], p1proj);
            origin2.H = origin2.G + Vector3.Distance(vertices[indices[t1 * 3 + 1]], p2proj);

            Node origin3 = new Node();
            origin3.index = indices[t1 * 3 + 2];
            origin3.parent = null;
            origin3.G = Vector3.Distance(vertices[indices[t1 * 3 + 2]], p1proj);
            origin3.H = origin3.G + Vector3.Distance(vertices[indices[t1 * 3 + 2]], p2proj);

            List<Node> closedList = new List<Node>();
            List<Node> openList = new List<Node>();

            openList.Add(origin1);
            openList.Add(origin2);
            openList.Add(origin3);
            Node fin = new Node();

            int passedThrough = 0;

            bool found = false;

            while (openList.Count != 0)
            {
                openList.Sort((x, y) => y.H.CompareTo(x.H));
                Node n = openList.Last();

                passedThrough++;

                //UnityEngine.Debug.Log("Cost of last:" +  n.H + ", cost of n-1:" + openList[openList.Count-2].H);



                //UnityEngine.Debug.Log("Looking at vertice index " + n.index);

                if (n.index == indices[t2 * 3 + 0] || n.index == indices[t2 * 3 + 1] || n.index == indices[t2 * 3 + 2])
                {
                    //UnityEngine.Debug.Log("FOUND IT!");
                    found = true;
                    fin = n;
                    break;
                }

                //UnityEngine.Debug.Log("Has " + pointGraphs[n.index].neighbours.Count + " neighbours.");
                foreach (int v in pointGraphs[n.index].neighbours)
                {
                    bool outing = false;
                    foreach (Node n2 in closedList)
                    {
                        if (n2.index == v) { outing = true; break; }
                    }
                    if (outing) continue;
                    float distance = Vector3.Distance(vertices[v], vertices[n.index]);
                    foreach (Node n2 in openList)
                    {
                        if (n2.index == v && n2.G <= n.G + distance) { outing = true; break; }
                        if (n2.index == v && n2.G > n.G + distance) { openList.Remove(n2); break; }
                    }
                    if (outing) continue;

                    //UnityEngine.Debug.Log("Ajout d'un node");

                    Node nv = new Node();
                    nv.index = v;
                    nv.parent = n;
                    nv.G = n.G + distance;
                    nv.H = nv.G + Vector3.Distance(vertices[v], p2proj);
                    openList.Add(nv);

                }
                openList.Remove(n);
                closedList.Add(n);
            }

            if (!found)
            {
                return path;
            }

            UnityEngine.Debug.Log("Passed through "+passedThrough + " of " + vertices.Length);



            // Reconstruction du chemin total;

            path.Add(p2proj);
            path.Add(vertices[fin.index]);
            while (fin.parent != null)
            {
                UnityEngine.Debug.Log(vertices[fin.index]);
                fin = fin.parent;
                path.Add(vertices[fin.index]);
            }
            path.Add(p1proj);
            path.Reverse();





            //DebugMesh(closedList, vertices, out vertices);
            // Step 3: funnel up;

            return path;

        }

        static int NearestTriangle(Vector3 [] vertices, int [] indices, Vector3 point, out Vector3 actualProjection)
        {

            int bestTriangle = 0;
            float bestDistance = 9999999;

            // Finds closest triangle
            for (int i = 0; i < indices.Length / 3; i++)
            {
                float distance = 0;
                distance = (ClosestPointOnTriangle(vertices[indices[i * 3 + 0]], vertices[indices[i * 3 + 1]], vertices[indices[i * 3 + 2]], point) - point).Length(); // LenghtSquared saves a squareroot each time;

                if (distance < bestDistance)
                {
                    bestTriangle = i;
                    bestDistance = distance;
                }

            }
            actualProjection = ClosestPointOnTriangle(vertices[indices[bestTriangle * 3 + 0]], vertices[indices[bestTriangle * 3 + 1]], vertices[indices[bestTriangle * 3 + 2]], point);
            return bestTriangle;
        }

        static float Clamp( float val, float min, float max)
        {
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }

        static float Abs(float val)
        {
            if (val < 0) return -val;
            return val;
        }

        static Vector3 ClosestPointOnTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 source)
        {
            Vector3 edge0 = v2 - v1;
            Vector3 edge1 = v3 - v1;
            Vector3 v0 = v1 - source;

            float a = Vector3.Dot(edge0, edge0);
            float b = Vector3.Dot(edge0, edge1);
            float c = Vector3.Dot(edge1, edge1);
            float d = Vector3.Dot(edge0, v0);
            float e = Vector3.Dot(edge1, v0);

            float det = Abs(a * c - b * b);
            float s = b * e - c * d;
            float t = b * d - a * e;

            if (s + t <= det)
            {
                if (s < 0.0f)
                {
                    if (t < 0.0f)
                    {
                        if (d < 0.0f)
                        {
                            s = Clamp(-d / a, 0.0f, 1.0f);
                            t = 0.0f;
                        }
                        else
                        {
                            s = 0.0f;
                            t = Clamp(-e / c, 0.0f, 1.0f);
                        }
                    }
                    else
                    {
                        s = 0.0f;
                        t = Clamp(-e / c, 0.0f, 1.0f);
                    }
                }
                else if (t < 0.0f)
                {
                    s = Clamp(-d / a, 0.0f, 1.0f);
                    t = 0.0f;
                }
                else
                {
                    float invDet = 1.0f / det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else
            {
                if (s < 0.0f)
                {
                    float tmp0 = b + d;
                    float tmp1 = c + e;
                    if (tmp1 > tmp0)
                    {
                        float numer = tmp1 - tmp0;
                        float denom = a - 2 * b + c;
                        s = Clamp(numer / denom, 0.0f, 1.0f);
                        t = 1 - s;
                    }
                    else
                    {
                        t = Clamp(-e / c, 0.0f, 1.0f);
                        s = 0.0f;
                    }
                }
                else if (t < 0.0f)
                {
                    if (a + d > b + e)
                    {
                        float numer = c + e - b - d;
                        float denom = a - 2 * b + c;
                        s = Clamp(numer / denom, 0.0f, 1.0f);
                        t = 1 - s;
                    }
                    else
                    {
                        s = Clamp(-e / c, 0.0f, 1.0f);
                        t = 0.0f;
                    }
                }
                else
                {
                    float numer = c + e - b - d;
                    float denom = a - 2 * b + c;
                    s = Clamp(numer / denom, 0.0f, 1.0f);
                    t = 1.0f - s;
                }
            }

            return v1 + s * edge0 + t * edge1;
        }

    }
}
