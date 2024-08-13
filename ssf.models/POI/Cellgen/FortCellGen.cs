using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.POI.Cellgen
{
    public static class FortCellGen
    {

        public static List<FormKey> SmallPkns = new List<FormKey>();
        public static float GetRot(int euler)
        {
            switch (euler)
            {
                case 0:
                    return 0;
                case 90:
                    return 1.57079632f;
                case 180:
                    return 3.14159f;
                case 270:
                    return 4.71239f;
            }
            return 0;
        }

        public static void BuildPkns(StarfieldMod myMod)
        {
            //Base
            for(int i = 0; i < myMod.PackIns.Count; i++)
            {
                if (myMod.PackIns.ElementAt(i).EditorID != null)
                {
                    if (myMod.PackIns.ElementAt(i).EditorID == "to_pkn_base")
                    {
                        SmallPkns.Add(myMod.PackIns.ElementAt(i).FormKey);
                    }
                    if (myMod.PackIns.ElementAt(i).EditorID.Contains("to_pkn_sm_"))
                    {
                        SmallPkns.Add(myMod.PackIns.ElementAt(i).FormKey);
                    }
                }
            }
        }

        //This function decides which packins to place and where in a single cell.
        //Will probably be a visitor pattern later?
        public static List<IPlaced> BuildCell(StarfieldMod myMod, int seed, P2Int cellPos)
        {
            var results = new List<IPlaced>();
            BuildPkns(myMod);
            //Add blocks
            //Cell sizes
            // 10,10,-10
            // 90,90,-10
            //Block offset matters
            //Block is 16

            int edgepadding = 10;
            int cellsize = 100;
            int blocksize = 16;

            Random rand = new Random(seed);

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    var inewblock = new PlacedObject(myMod)
                    {
                        Base = SmallPkns.ElementAt(rand.Next(SmallPkns.Count)).ToNullableLink<IPlaceableObjectGetter>(),
                        Position = new P3Float((cellPos.X * cellsize) + (edgepadding + (blocksize * x)), (cellPos.Y * cellsize) + edgepadding + (blocksize * y), -10),
                        Rotation = new P3Float(0, 0, GetRot(rand.Next(3)*90)),
                        MajorRecordFlagsRaw = 66560
                    };
                    results.Add(inewblock);
                }
            }
            return results;
        }
    }
}
