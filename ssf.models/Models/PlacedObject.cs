using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    public class PlacedObject
    {
        public PlacedObject()
        {
            if (Scale == 0)
            {
                Scale = 1;
            }
        }
        public string MutagenObjectType { get; set; }
        public string FormKey { get; set; }
        public string EditorID { get; set; }
        public int FormVersion { get; set; }
        public int VersionControl { get; set; }
        public string Base { get; set; }

        public float Scale { get; set; }
        public Placement Placement { get; set; }

        //Stuff i'm not using:
        public int MajorRecordFlagsRaw { get; set; }
        public List<int> SkyrimMajorRecordFlags { get; set; }
        public float Radius { get; set; }
        public LightData LightData { get; set; }
        public string LevelModifier { get; set; }
    }

    public class Placement
    {
        public string Position { get; set; }
        public string Rotation { get; set; }
    }

    public class LightData
    {
        public double FadeOffset {  get; set; }
        public double ShadowDepthBias { get; set; }
    }

}
