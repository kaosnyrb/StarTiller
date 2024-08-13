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
        //This function decides which packins to place and where in a single cell.
        //Will probably be a visitor pattern later?
        public static List<IPlaced> BuildCell(StarfieldMod myMod, int seed, P2Int cellPos)
        {
            var results = new List<IPlaced>();
            //Add blocks
            //Cell sizes
            // 10,10,-10
            // 90,90,-10
            //Block offset matters
            //Block is 16

            int edgepadding = 8;
            int cellsize = 100;
            int blocksize = 16;

            IFormLinkNullable<IPlaceableObjectGetter> to_pkn_base = new FormKey(myMod.ModKey, 0x000014BE).ToNullableLink<IPlaceableObjectGetter>();
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    var inewblock = new Mutagen.Bethesda.Starfield.PlacedObject(myMod)
                    {
                        Base = to_pkn_base,
                        Position = new P3Float((cellPos.X * cellsize) + (edgepadding + (blocksize * x)), (cellPos.Y * cellsize) + edgepadding + (blocksize * y), -10)
                    };
                    results.Add(inewblock);
                }
            }
            return results;
        }
    }
}
