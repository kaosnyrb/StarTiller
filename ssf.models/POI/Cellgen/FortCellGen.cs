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

        public static Dictionary<string, List<FormKey>> packinLib = new Dictionary<string, List<FormKey>>();
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

        public static List<FormKey> GetPackinFormsForId(StarfieldMod myMod, string id)
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
                { "to_pkn_wall_", GetPackinFormsForId(myMod, "to_pkn_wall_") },
                { "to_pkn_wallcorner_", GetPackinFormsForId(myMod, "to_pkn_wallcorner_") },
                { "to_pkn_wallinside_", GetPackinFormsForId(myMod, "to_pkn_wallinside_") },
                { "to_pkn_single", GetPackinFormsForId(myMod, "to_pkn_single") },
                { "to_pkn_large", GetPackinFormsForId(myMod, "to_pkn_large") },
                { "to_pkn_small_", GetPackinFormsForId(myMod, "to_pkn_small_") }
            };
        }

        //This designs the layout
        public static void BuildMap(StarfieldMod myMod, int seed)
        {
            //Load the packins
            BuildPkns(myMod);
            Random rand = new Random(seed);
            //Generate the map
            //A small block is 3x3
            //Cells are
            // 0-36
            // 37-72
            int mapsize = 50;
            map = new GenerationMap(mapsize, mapsize);

            //Place the roads
            int roadcount = 3 + rand.Next(3);

            int startx = rand.Next(3) * 3;
            int starty = rand.Next(3) * 3;
            int length = (6 + rand.Next(10)) * 3;

            for (int i = 0; i < length * 2; i += 3)
            {
                map.placesmalltileonempty(startx + i, starty, "to_pkn_base", 0, "floor");
                map.placesmalltileonempty(startx + i, starty + 3, "to_pkn_base", 0, "floor");
                map.placesmalltileonempty(startx + i, starty - 3, "to_pkn_base", 0, "floor");
                map.placesmalltileonempty(startx + i, starty + 6, "to_pkn_base", 0, "floor");
                map.placesmalltileonempty(startx + i, starty - 6, "to_pkn_base", 0, "floor");
            }

            for (int road = 0; road < roadcount; road++)
            {
                //Place X road
                startx += 3 + (rand.Next(3) * 3);
                for (int i = 0; i < length; i += 3)
                {
                    map.placesmalltileonempty(startx, starty + i, "to_pkn_base", 0, "floor");
                    map.placesmalltileonempty(startx + 3, starty + i, "to_pkn_base", 0, "floor");
                    map.placesmalltileonempty(startx - 3, starty + i, "to_pkn_base", 0, "floor");
                }
            }

            //Place single filler blocks
            //Having a single strip of empty blocks looks wierd
            for (int x = 1; x < mapsize - 1; x++)
            {
                for (int y = 1; y < mapsize - 1; y++)
                {
                    if (map.tiles[x][y].type == "empty")
                    {
                        if (map.tiles[x - 1][y].type == "floor" && map.tiles[x + 1][y].type == "floor")
                        {
                            map.placesingletile(x, y, "to_pkn_single", 0);
                        }
                        if (map.tiles[x][y - 1].type == "floor" && map.tiles[x][y + 1].type == "floor")
                        {
                            map.placesingletile(x, y, "to_pkn_single", 0);
                        }
                    }
                }
            }

            //Build walls
            for (int x = 3; x < mapsize - 3; x++)
            {
                for (int y = 3; y < mapsize - 3; y++)
                {
                    if (map.tiles[x][y].type == "to_pkn_base")
                    {
                        //Check surrounding to see if we should build a wall

                        bool topclear = false;
                        bool botclear = false;
                        bool leftclear = false;
                        bool rightclear = false;

                        bool tlclear = false;
                        bool trclear = false;
                        bool blclear = false;
                        bool brclear = false;

                        if (map.tiles[x][y - 3].type == "empty")
                        {
                            topclear = true;
                        }
                        if (map.tiles[x][y + 3].type == "empty")
                        {
                            botclear = true;
                        }
                        if (map.tiles[x - 3][y].type == "empty")
                        {
                            leftclear = true;
                        }
                        if (map.tiles[x + 3][y].type == "empty")
                        {
                            rightclear = true;
                        }
                        if (map.tiles[x - 3][y - 3].type == "empty")
                        {
                            tlclear = true;
                        }
                        if (map.tiles[x + 3][y - 3].type == "empty")
                        {
                            trclear = true;
                        }
                        if (map.tiles[x - 3][y + 3].type == "empty")
                        {
                            blclear = true;
                        }
                        if (map.tiles[x + 3][y + 3].type == "empty")
                        {
                            brclear = true;
                        }

                        bool placed = false;
                        if (topclear && leftclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallcorner_", 0, "ignore");
                            placed = true;
                        }
                        if (topclear && rightclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallcorner_", 90, "ignore");
                            placed = true;
                        }
                        if (botclear && rightclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallcorner_", 180, "ignore");
                            placed = true;
                        }
                        if (botclear && leftclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallcorner_", 270, "ignore");
                            placed = true;
                        }
                        if (topclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wall_", 0, "ignore");
                            placed = true;
                        }
                        if (botclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wall_", 180, "ignore");
                            placed = true;
                        }
                        if (leftclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wall_", 270, "ignore");
                            placed = true;
                        }
                        if (rightclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wall_", 90, "ignore");
                            placed = true;
                        }
                        if(tlclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallinside_", 0, "ignore");
                            placed = true;
                        }
                        if (trclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallinside_", 90, "ignore");
                            placed = true;
                        }
                        if (brclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallinside_", 180, "ignore");
                            placed = true;
                        }
                        if (blclear && !placed)
                        {
                            //Corner
                            map.placesmalltile(x, y, "to_pkn_wallinside_", 270, "ignore");
                            placed = true;
                        }
                    }
                }
            }

            //Place large blocks over bases
            int largeblockcount = 2 + rand.Next(5);
            int attempts = 100;//Breakout in case being stuck
            for (int i = 0; i < largeblockcount; i++)
            {
                bool foundblock = false;
                for (int x = 3; x < mapsize - 3; x++)
                {
                    for (int y = 3; y < mapsize - 3; y++)
                    {
                        //Find random road block
                        if (map.tiles[x][y].type == "to_pkn_base" && !foundblock)
                        {
                            //Check surroundings
                            if (map.tiles[x + 3][y].type == "to_pkn_base" &&
                                map.tiles[x + 3][y + 3].type == "to_pkn_base" &&
                                map.tiles[x + 3][y - 3].type == "to_pkn_base" &&
                                map.tiles[x - 3][y].type == "to_pkn_base" &&
                                map.tiles[x - 3][y + 3].type == "to_pkn_base" &&
                                map.tiles[x - 3][y - 3].type == "to_pkn_base" &&
                                map.tiles[x][y + 3].type == "to_pkn_base" &&
                                map.tiles[x][y - 3].type == "to_pkn_base")
                            {
                                //We don't want it to be just first come first serve.
                                if (rand.Next(100) > 25)
                                {
                                    //Convert the group of 9 small bases into a large base.
                                    map.placelargetile(x, y, "to_pkn_large", rand.Next(3) * 90, "floor");
                                    foundblock = true;
                                }
                            }
                        }
                    }
                }
            }

            //Place small blocks over bases
            int blockcount = 10 + rand.Next(10);
            attempts = 1500;
            for (int i = 0; i < blockcount; i++)
            {
                bool foundblock = false;
                int x = rand.Next(mapsize);
                int y = rand.Next(mapsize);
                while (!foundblock && attempts > 0)
                {
                    attempts--;
                    //Find random road block
                    if (map.tiles[x][y].type == "to_pkn_base")
                    {
                        map.placesmalltile(x, y, "to_pkn_small_", rand.Next(3) * 90, "floor");
                        foundblock = true;
                    }
                    else
                    {
                        x = rand.Next(mapsize);
                        y = rand.Next(mapsize);
                    }
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
                for (int y = starty; y < endy; y++)
                {
                    if (map.tiles[x][y].type != "empty" && map.tiles[x][y].type != "full")
                    {
                        //The tile isn't empty or blocked off by another prefab
                        if (packinLib.ContainsKey(map.tiles[x][y].type))
                        {
                            var prefab = packinLib[map.tiles[x][y].type].ElementAt(rand.Next(packinLib[map.tiles[x][y].type].Count));
                            P3Float pos = new P3Float(-94 + (blocksize * x), 94 - (blocksize * y), -10);

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
