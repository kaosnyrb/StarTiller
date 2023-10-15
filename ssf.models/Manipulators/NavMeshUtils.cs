using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Manipulators
{
    public class NavMeshUtils
    {
        public static Navmesh BuildNavmesh(List<Block> blocks)
        {
            SSFEventLog.EventLogs.Enqueue("Building Navmesh. Be patient as this can take a min or two.");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int distancecheck = 512;

            Navmesh mesh = blocks[0].navmeshs[0];

            List<string> vertices = new List<string>();
            List<Triangle> triangles = new List<Triangle>();

            int mergedVerts = 0;
            foreach (var block in blocks)
            {
                foreach (var nav in block.navmeshs)
                {
                    //Original Vert / New Vert. Used to transform block triangles into global triangles
                    Dictionary<int, int> VertTransformation = new Dictionary<int, int>();
                    //Add Verts
                    for (int i = 0; i < nav.Data.Vertices.Length;i++)
                    {
                        Vector3 vert = Utils.ConvertStringToVector3(nav.Data.Vertices[i]);

                        //Merge
                        bool Merged = false;
                        int MergeVert = -1;
                        for(int j = 0; j < vertices.Count; j++)
                        {
                            Vector3 testvert = Utils.ConvertStringToVector3(vertices[j]);
                            Vector3 distance = (vert - testvert);
                            if (distance.LengthSquared() <= distancecheck)
                            {
                                Merged = true;
                                MergeVert = j;
                            }
                        }
                        //Output
                        if (Merged)
                        {
                            VertTransformation.Add(i, MergeVert);
                            mergedVerts++;
                        }
                        else
                        {
                            vertices.Add(Utils.ConvertVector3ToString(vert));
                            int position = vertices.Count - 1;
                            VertTransformation.Add(i, position);
                        }
                    }
                    //Add Triangles
                    for(int i = 0; i < nav.Data.Triangles.Length; i++)
                    {
                        Triangle triangle = nav.Data.Triangles[i];
                        Vector3 triverts = Utils.ConvertStringToVector3(triangle.Vertices);
                        triverts.X = VertTransformation[(int)triverts.X];
                        triverts.Y = VertTransformation[(int)triverts.Y];
                        triverts.Z = VertTransformation[(int)triverts.Z];
                        triangle.Vertices = Utils.ConvertVector3ToString(triverts);

                        if (triangle.EdgeLink_0_1 >= 0 && VertTransformation.ContainsKey(triangle.EdgeLink_0_1))
                        {
                            triangle.EdgeLink_0_1 = VertTransformation[triangle.EdgeLink_0_1];
                        }
                        if (triangle.EdgeLink_2_0 >= 0 && VertTransformation.ContainsKey(triangle.EdgeLink_2_0))
                        {
                            triangle.EdgeLink_2_0 = VertTransformation[triangle.EdgeLink_2_0];
                        }
                        if (triangle.EdgeLink_1_2 >= 0 && VertTransformation.ContainsKey(triangle.EdgeLink_1_2))
                        {
                            triangle.EdgeLink_1_2 = VertTransformation[triangle.EdgeLink_1_2];
                        }
                        triangles.Add(triangle);
                    }
                }
            }
            mesh.Data = new Data
            {
                Vertices = vertices.ToArray(),
                Triangles = triangles.ToArray(),
                Min = Utils.ConvertVector3ToString(new Vector3(0, 0, 0)),
                Max = Utils.ConvertVector3ToString(new Vector3(0, 0, 0)),
                //NavmeshGrid is replaced in the construction set, don't bother building it here.
                NavmeshGrid = "0x03000000030004000500080000000000010008000A000C000D000E0013000C00000002000300040005000600070009000B000F00100011001200070000000000010005000C000D000E001000",
                Parent = new parclass(),
                MaxDistanceX = 0.1f,
                MaxDistanceY = -0.1f,
                NavmeshGridDivisor = 1
            };

            watch.Stop();
            var elapsed = watch.Elapsed;
            SSFEventLog.EventLogs.Enqueue("Navmesh Complete, Time taken: " + elapsed );
            SSFEventLog.EventLogs.Enqueue("Vertices: " + mesh.Data.Vertices.Length);
            SSFEventLog.EventLogs.Enqueue("Vertices Merged: " + mergedVerts);
            SSFEventLog.EventLogs.Enqueue("Triangles: " + mesh.Data.Triangles.Length);
            return mesh;
        }


    }
}
