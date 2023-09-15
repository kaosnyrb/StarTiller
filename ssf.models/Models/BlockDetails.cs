using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    public class BlockDetails
    {
        public Vector3 startpoint { get; set; }
        public string startConnector { get; set; }
        public string blocktype { get; set; }
        public List<Connector> Connectors { get; set; }
        public Vector3 BoundingTopLeft { get; set; }
        public Vector3 BoundingBottomRight { get; set; }   
    }

    public class Connector
    {
        public Vector3 startpoint { get; set; }
        public float rotation {  get; set; }

        public string connectorName { get; set; }
    }
}
