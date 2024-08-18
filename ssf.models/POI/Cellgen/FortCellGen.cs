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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ssf.POI.Cellgen
{
    public static class FortCellGen
    {

        public static Dictionary<string, List<FormKey>> packinLib = new Dictionary<string, List<FormKey>>();
        public static GenerationMap map;

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
                { "to_pkn_gate_", GetPackinFormsForId(myMod, "to_pkn_gate_") },
                { "to_pkn_single", GetPackinFormsForId(myMod, "to_pkn_single") },
                { "to_pkn_large", GetPackinFormsForId(myMod, "to_pkn_large") },
                { "to_pkn_landing_", GetPackinFormsForId(myMod, "to_pkn_landing_") },
                { "to_pkn_small_", GetPackinFormsForId(myMod, "to_pkn_small_") }
            };
        }

        //This designs the layout
        public static void BuildMap(StarfieldMod myMod, int seed)
        {
            //Load the packins
            BuildPkns(myMod);
            Random rand = new Random(seed);
            int mapsize = 50;
            map = new GenerationMap(mapsize, mapsize);
            //Place the rects around the center point
            //We draw a bunch of rectangles from a central point  so they overlap in the middle.
            //This is like blocky flower petals
            int centerx = 24;
            int centery = 24;
                        
            //Landing Pads are kinda cool, but dominate the size of smaller pois.
            if (  rand.Next(100) > 75 )
            {
                //Core of 12
                for (int x = centerx - 6; x <= centerx + 6; x += 3)
                {
                    for (int y = centery - 6; y <= centery + 6; y += 3)
                    {
                        map.placesmalltileonempty(x, y, "to_pkn_base", 0, "floor");
                    }
                }
                //Place LandingPad
                map.placelandingpadtile(centerx, centery, "to_pkn_landing_", 0, "pad");
            }
            else
            {
                //Core of 9
                for (int x = centerx - 3; x <= centerx + 3; x += 3)
                {
                    for (int y = centery - 3; y <= centery + 3; y += 3)
                    {
                        map.placesmalltileonempty(x, y, "to_pkn_base", 0, "floor");
                    }
                }
            }

            int numberofrects = 3 + rand.Next(4);
            for (int i = 0;i  < numberofrects;i++)
            {
                int direction = rand.Next(4);

                int rectx = (2 + rand.Next(5)) * 3;
                int recty = (2 + rand.Next(5)) * 3;
                switch(direction)
                {
                    case 0:
                        //TopLeft
                        for (int x = centerx - rectx; x <= centerx; x += 3)
                        {
                            for (int y = centery - recty; y <= centery; y += 3)
                            {
                                map.placesmalltileonempty(x, y, "to_pkn_base", 0, "floor");
                            }
                        }
                        break;
                    case 1:
                        //TopRight
                        for (int x = centerx; x <= centerx + rectx; x += 3)
                        {
                            for (int y = centery - recty; y <= centery; y += 3)
                            {
                                map.placesmalltileonempty(x, y, "to_pkn_base", 0, "floor");
                            }
                        }
                        break;
                    case 2:
                        //BottomRight
                        for (int x = centerx; x <= centerx + rectx; x += 3)
                        {
                            for (int y = centery; y <= centery + recty; y += 3)
                            {
                                map.placesmalltileonempty(x, y, "to_pkn_base", 0, "floor");
                            }
                        }
                        break;
                    case 3:
                        //BottomLeft
                        for (int x = centerx - rectx; x <= centerx; x += 3)
                        {
                            for (int y = centery; y <= centery + recty; y += 3)
                            {
                                map.placesmalltileonempty(x, y, "to_pkn_base", 0, "floor");
                            }
                        }
                        break;
                }
            }

            //Place single filler blocks
            //Having a single strip of empty blocks looks wierd
            for (int x = 1; x < mapsize - 1; x++)
            {
                for (int y = 1; y < mapsize - 1; y++)
                {
                    if (map.tiles[x][y].prefabs.Count == 0)
                    {
                        if (map.tiles[x - 1][y].prefabs.Contains("floor") && map.tiles[x + 1][y].prefabs.Contains("floor"))
                        {
                            map.placesingletile(x, y, "to_pkn_single", 0);
                        }
                        if (map.tiles[x][y - 1].prefabs.Contains("floor") && map.tiles[x][y + 1].prefabs.Contains("floor"))
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
                    if (map.tiles[x][y].prefabs.Contains("to_pkn_base"))
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

                        if (map.tiles[x][y - 3].prefabs.Count == 0)
                        {
                            topclear = true;
                        }
                        if (map.tiles[x][y + 3].prefabs.Count == 0)
                        {
                            botclear = true;
                        }
                        if (map.tiles[x - 3][y].prefabs.Count == 0)
                        {
                            leftclear = true;
                        }
                        if (map.tiles[x + 3][y].prefabs.Count == 0)
                        {
                            rightclear = true;
                        }
                        if (map.tiles[x - 3][y - 3].prefabs.Count == 0)
                        {
                            tlclear = true;
                        }
                        if (map.tiles[x + 3][y - 3].prefabs.Count == 0)
                        {
                            trclear = true;
                        }
                        if (map.tiles[x - 3][y + 3].prefabs.Count == 0)
                        {
                            blclear = true;
                        }
                        if (map.tiles[x + 3][y + 3].prefabs.Count == 0)
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
            //Place gates on walls
            int gateloops = 100;
            int gatecount = 2 + rand.Next(2);

            List<int> roations = new List<int>();
            for(int i = 0; i < gateloops && gatecount > 0; i++)
            {
                for (int x = 3; x < mapsize - 3; x++)
                {
                    for (int y = 3; y < mapsize - 3; y++)
                    {
                        if (map.tiles[x][y].prefabs.Contains("to_pkn_wall_"))
                        {
                            //We don't want it to be just first come first serve.
                            if (rand.Next(100) < 10 && gatecount > 0 && !roations.Contains(map.tiles[x][y].rotation))
                            {
                                //Convert the wall into a gate, keeping rotation
                                roations.Add(map.tiles[x][y].rotation);//We only want 1 gate a side
                                map.placesmalltile(x, y, "to_pkn_gate_", map.tiles[x][y].rotation, "ignore");
                                gatecount--;
                            }
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
                        if (map.tiles[x][y].prefabs.Contains("to_pkn_base") && !foundblock)
                        {
                            //Check surroundings
                            if (map.tiles[x + 3][y].prefabs.Contains("to_pkn_base") &&
                                map.tiles[x + 3][y + 3].prefabs.Contains("to_pkn_base") &&
                                map.tiles[x + 3][y - 3].prefabs.Contains("to_pkn_base") &&
                                map.tiles[x - 3][y].prefabs.Contains("to_pkn_base") &&
                                map.tiles[x - 3][y + 3].prefabs.Contains("to_pkn_base") &&
                                map.tiles[x - 3][y - 3].prefabs.Contains("to_pkn_base") &&
                                map.tiles[x][y + 3].prefabs.Contains("to_pkn_base") &&
                                map.tiles[x][y - 3].prefabs.Contains("to_pkn_base"))
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
            int blockcount = 1000 + rand.Next(10);
            attempts = 150000;
            for (int i = 0; i < blockcount; i++)
            {
                bool foundblock = false;
                int x = rand.Next(mapsize);
                int y = rand.Next(mapsize);
                while (!foundblock && attempts > 0)
                {
                    attempts--;
                    //Find random road block
                    if (map.tiles[x][y].prefabs.Contains("to_pkn_base"))
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
                    if (map.tiles[x][y].prefabs.Count > 0)
                    {
                        //The tile isn't empty or blocked off by another prefab
                        foreach(var pfb in map.tiles[x][y].prefabs)
                        {
                            if (packinLib.ContainsKey(pfb))
                            {
                                int prefabid = rand.Next(packinLib[pfb].Count);
                                var prefab = packinLib[pfb].ElementAt(prefabid);
                                if (!myMod.PackIns[prefab].EditorID.Contains("reuse"))
                                {
                                    //We should have reuseable versions of every piece but don't crash.
                                    if (packinLib[pfb].Count > 1)
                                    {
                                        packinLib[pfb].RemoveAt(prefabid);
                                    }
                                }
                                float z = -10;
                                if (map.tiles[x][y].zoverride != 0)
                                {
                                    z = map.tiles[x][y].zoverride;
                                }
                                P3Float pos = new P3Float(-94 + (blocksize * x), 94 - (blocksize * y), z);

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
            }

            return results;
        }
    }
}
