using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{

    public class Navmesh
    {
        public string FormKey { get; set; }
        public int FormVersion { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string[] Vertices { get; set; }
        public Triangle[] Triangles { get; set; }
        public int NavmeshGridDivisor { get; set; }
        public float MaxDistanceX { get; set; }
        public float MaxDistanceY { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public object NavmeshGrid { get; set; }
    }

    public class Triangle
    {
        public string Vertices { get; set; }
        public int EdgeLink_0_1 { get; set; }
        public int EdgeLink_1_2 { get; set; }
        public int EdgeLink_2_0 { get; set; }
        public string[] Flags { get; set; }
    }

}
