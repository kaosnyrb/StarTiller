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

        public void translate(Vector3 Pivot, Vector3 translation, float rotation)
        {
            //Convert string to vector3
            var pos = Utils.ConvertStringToVector3(Position);

            //Rotate around pivot 
            pos = Utils.RotateVectorAroundPivot(Pivot, pos, rotation);

            //Apply translation
            pos += translation;

            //convert back to string (needed for export)
            Position = Utils.ConvertVector3ToString(pos);

            //Apply Rotation
            var rot = Utils.ConvertStringToVector3(Rotation);


            //TODO
            //;Objects in Skyrim are rotated in order of Z, Y, X, so we will do that here as well.
            // Y and X change if Z do.
            var result = Utils.LocalToGlobal(rot, new Vector3(0, 0, Utils.ToRadians(-rotation)));

            Rotation = Utils.ConvertVector3ToString(result);
        }
    }

    public class LightData
    {
        public double FadeOffset {  get; set; }
        public double ShadowDepthBias { get; set; }
    }

}
