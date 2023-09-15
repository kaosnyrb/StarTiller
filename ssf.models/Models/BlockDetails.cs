using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    public class BlockDetails
    {
        public Vector3 startpoint { get; set; }
        public List<Vector4> Connectors { get; set; }
        public Vector3 BoundingTopLeft { get; set; }
        public Vector3 BoundingBottomRight { get; set; }   
    }
}
