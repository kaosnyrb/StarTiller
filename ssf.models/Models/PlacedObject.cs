using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    public class PlacedObject
    {
        public string MutagenObjectType { get; set; }
        public string FormKey { get; set; }
        public string EditorID { get; set; }
        public int FormVersion { get; set; }
        public string Base { get; set; }
        public float Scale { get; set; }
        public Placement Placement { get; set; }
    }

    public class Placement
    {
        public string Position { get; set; }
        public string Rotation { get; set; }
    }
}
