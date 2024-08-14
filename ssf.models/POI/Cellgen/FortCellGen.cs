using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;
using ssf.Models;
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

        public static Dictionary<string,List<FormKey>> packinLib = new Dictionary<string, List<FormKey>>();
        public static GenerationMap map;

        //public static List<FormKey> SmallPkns = new List<FormKey>();
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

        public static List<FormKey>GetPackinFormsForId(StarfieldMod myMod,string id)
        {
            List<FormKey> to_pkn_base = new List<FormKey>();
            for (int i = 0; i < myMod.PackIns.Count; i++)
            {
                if (myMod.PackIns.ElementAt(i).EditorID != null)
                {
                    if (myMod.PackIns.ElementAt(i).EditorID.Contains(id))
                    {
                        to_pkn_base.Add(myMod.PackIns.ElementAt(i).FormKey);
                    }
                }
            }
            return to_pkn_base;
        }

        public static void BuildPkns(StarfieldMod myMod)
        {
            packinLib = new Dictionary<string, List<FormKey>>
            {
                { "to_pkn_base", GetPackinFormsForId(myMod, "to_pkn_base") },
                { "to_pkn_sm_", GetPackinFormsForId(myMod, "to_pkn_sm_") }
            };
        }

        //This designs the layout
        public static void BuildMap(StarfieldMod myMod, int seed)
        {
            //Load the packins
            BuildPkns(myMod);

            //Generate the map
            //A small block is 3x3
            //Cells are
            // 0-36
            // 37-72
            map = new GenerationMap(50, 50);
            //map.placesmalltile(48,48, "to_pkn_base", 0);
            
            for(int x = 0; x < 16; x++)
            {
                for(int y = 0;  y < 16; y++)
                {
                    map.placesmalltile(1 + (x * 3), 1 + (y * 3), "to_pkn_base", 0);
                }
            }
        }

        //This function takes a map and builds one of four cells
        public static List<IPlaced> BuildCell(StarfieldMod myMod, int seed, P2Int cellPos)
        {
            var results = new List<IPlaced>();
            int edgepadding = 2;
            int cellsize = 100;
            int blocksize = 4;
            Random rand = new Random(seed);


            //This function builds 4 cells from one map, need to build 1 quadrant based on cellPos
            int startx = 0;
            int starty = 0;
            int endx = map.xsize;
            int endy = map.ysize;

            //Yeah -1 is left of 0
            if (cellPos.X == -1)
            {
                startx = 0;
                endx = (map.xsize / 2);
            }
            if (cellPos.X == 0)
            {
                startx = (map.xsize / 2);
                endx = map.xsize;
            }
            if (cellPos.Y == 0)
            {
                starty = 0;
                endy = (map.ysize / 2);
            }
            if (cellPos.Y == -1)
            {
                starty = (map.ysize / 2);
                endy = map.ysize;
            }


            for (int x = startx; x < endx; x++)
            {
                for(int y = starty; y < endy; y++)
                {
                    if (map.tiles[x][y].type != "empty" && map.tiles[x][y].type != "full")
                    {
                        //The tile isn't empty or blocked off by another prefab
                        if (packinLib.ContainsKey(map.tiles[x][y].type))
                        {
                            var prefab = packinLib[map.tiles[x][y].type].ElementAt(rand.Next(packinLib[map.tiles[x][y].type].Count));
                            P3Float pos = new P3Float(-98 + (blocksize * x),
                                98 - (blocksize * y),
                                -10);
                            if (cellPos.X == -1)
                            {
                                pos.X = -94 + (blocksize * x);
                            }
                            else
                            {
                                pos.X = -94 + (blocksize * x);
                            }
                            if(cellPos.Y == -1)
                            {
                                pos.Y = 94 - (blocksize * y);
                            }
                            else
                            {
                                pos.Y = 94 - (blocksize * y);
                            }
                            
                            
                            var inewblock = new PlacedObject(myMod)
                            {
                                Base = prefab.ToNullableLink<IPlaceableObjectGetter>(),
                                Position = pos,
                                Rotation = new P3Float(0, 0, GetRot(map.tiles[x][y].rotation)),
                                MajorRecordFlagsRaw = 66560
                            };
                            results.Add(inewblock);
                        }
                    }
                }
            }
            return results;
        }
    }
}
