﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{

    public class Navmesh
    {
        public string FormKey { get; set; }
        public int FormVersion { get; set; }
        public Data Data { get; set; }
        public int VersionControl {  get; set; } 

        
        public void translate(Vector3 Pivot, Vector3 translation, float rotation)
        {
            for ( int i = 0; i < Data.Vertices.Length; i++)
            {
                //Convert string to vector3
                var pos = Utils.ConvertStringToVector3(Data.Vertices[i]);

                //Rotate around pivot 
                pos = Utils.RotateVectorAroundPivot(Pivot, pos, rotation);

                //Apply translation
                pos += translation;

                //convert back to string (needed for export)
                Data.Vertices[i] = Utils.ConvertVector3ToString(pos);
            }
        }
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
        public parclass Parent { get; set; }
    }

    public class Triangle
    {
        public string Vertices { get; set; }
        public int EdgeLink_0_1 { get; set; }
        public int EdgeLink_1_2 { get; set; }
        public int EdgeLink_2_0 { get; set; }
        public string[] Flags { get; set; }

        public int CoverFlags {  get; set; }
        public bool IsCover { get; set; }
    }

    public class parclass
    {
        public string MutagenObjectType { get; set; }
        public string Parent { get; set; }
    }

}
