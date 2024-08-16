using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Starfield;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    public class tile
    {
        public string type = "empty";
        public int rotation = 0;
        public float zoverride = 0;
    }
    public  class GenerationMap
    {
        public tile[][] tiles;
        public int xsize;
        public int ysize;

        public GenerationMap(int x, int y) 
        { 
            xsize = x;
            ysize = y;

            tiles = new tile[x][];
            for (int i = 0; i < xsize; i++)
            {
                tiles[i] = new tile[ysize];
                for(int j = 0; j < ysize; j++)
                {
                    tiles[i][j] = new tile();
                }
            }
        }

        public bool canPlace(int x, int y)
        {
            if (x > xsize - 3 || x < 1 || y > ysize - 3 || y < 1)
            {
                //Doesn't fit
                return false;
            }
            if (tiles[x][y].type != "empty")return false;
            if (tiles[x][y + 1].type != "empty") return false;
            if (tiles[x][y - 1].type != "empty") return false;

            if (tiles[x + 1][y].type != "empty") return false;
            if (tiles[x + 1][y + 1].type != "empty") return false;
            if (tiles[x + 1][y - 1].type != "empty") return false;

            
            if (tiles[x - 1][y].type != "empty") return false;
            if (tiles[x - 1][y + 1].type != "empty") return false;
            if (tiles[x - 1][y - 1 ].type != "empty") return false;

            return true;
        }

        public bool placesmalltileonempty(int x, int y, string type, int rotation, string filltag)
        {
            bool debug = false;
            if (debug)
            {
                SSFEventLog.EventLogs.Enqueue("placesmalltileonempty " + x +" | " + y + " : " +type);
            }
            if (canPlace(x,y))
            {
                return placesmalltile(x,y,type,rotation,filltag);            
            }
            return false;
        }

        public bool placesingletile(int x, int y,string type,int rotation)
        {
            if (x > xsize || x < 0 || y > ysize || y < 0)
            {
                //Doesn't fit
                return false;
            }
            tiles[x][y].rotation = rotation;
            tiles[x][y].type = type;
            return true;
        }

        public bool placelargetile(int x, int y, string type, int rotation, string filltag)
        {
            if (x > xsize - 6 || x < 6 || y > ysize - 6 || y < 6)
            {
                //Doesn't fit
                return false;
            }

            //Do we care about overlaps?
            //For now don't worry.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tiles[(x-4)+i][(y-4) + j].rotation = 0;
                    tiles[(x-4) + i][(y-4) + j].type = filltag;
                }
            }
            //Place the centre tile
            tiles[x][y].rotation = rotation;
            tiles[x][y].type = type;
            return true;
        }

        public bool placelandingpadtile(int x, int y, string type, int rotation, string filltag)
        {
            if (x > xsize - 6 || x < 6 || y > ysize - 6 || y < 6)
            {
                //Doesn't fit
                return false;
            }

            //Do we care about overlaps?
            //For now don't worry.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    tiles[(x - 4) + i][(y - 4) + j].rotation = 0;
                    tiles[(x - 4) + i][(y - 4) + j].type = filltag;
                }
            }
            //Place the centre tile
            tiles[x][y].rotation = rotation;
            tiles[x][y].type = type;
            tiles[x][y].zoverride = -13.1000f;

            return true;
        }

        //squares of 3
        public void placeSquareofsmalltiles(int size, int centerx, int centery, string type, string filltag)
        {
            
            for (int x = centerx - (size / 3); x <= centerx + (size / 3); x += 3)
            {
                for (int y = centery - (size / 3); y <= centery + (size / 3); y += 3)
                {
                    placesmalltileonempty(x, y, type, 0, "floor");
                }
            }
        }

        public bool placesmalltile(int x, int y, string type, int rotation, string filltag)
        {
            if (x > xsize - 3 || x < 1 || y > ysize - 3 || y < 1)
            {
                //Doesn't fit
                return false;
            }

            //Do we care about overlaps?
            //For now don't worry.

            //Place the centre tile
            tiles[x][y].rotation = rotation;
            tiles[x][y].type = type;

            //Fill in the other tiles
            //Col 1
            tiles[x-1][y].rotation = 0;
            tiles[x-1][y].type = filltag;

            tiles[x-1][y-1].rotation = 0;
            tiles[x-1][y-1].type = filltag;

            tiles[x - 1][y + 1].rotation = 0;
            tiles[x - 1][y + 1].type = filltag;
            //Col 2
            tiles[x][y - 1].rotation = 0;
            tiles[x][y - 1].type = filltag;

            tiles[x][y + 1].rotation = 0;
            tiles[x][y + 1].type = filltag;

            //Col 3
            tiles[x + 1][y].rotation = 0;
            tiles[x + 1][y].type = filltag;

            tiles[x + 1][y - 1].rotation = 0;
            tiles[x + 1][y - 1].type = filltag;

            tiles[x + 1][y + 1].rotation = 0;
            tiles[x + 1][y + 1].type = filltag;

            return true;
        }
    }
}
