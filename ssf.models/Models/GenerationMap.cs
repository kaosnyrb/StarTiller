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
        public List<string> prefabs = new List<string>();
        //public string type = "empty";
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

        public void replacetile(int x, int y, string type, int rotation)
        {
            if (!tiles[x][y].prefabs.Contains(type))
            {
                tiles[x][y].prefabs.Clear();
                tiles[x][y].rotation = rotation;
                tiles[x][y].prefabs.Add(type);
            }
        }

        public void addontile(int x, int y, string type, int rotation)
        {
            if (!tiles[x][y].prefabs.Contains(type))
            {
                tiles[x][y].prefabs.Add(type);
            }
        }

        public bool canPlace(int x, int y)
        {
            if (x > xsize - 3 || x < 1 || y > ysize - 3 || y < 1)
            {
                //Doesn't fit
                return false;
            }
            if (tiles[x][y].prefabs.Count > 0)return false;
            if (tiles[x][y + 1].prefabs.Count > 0) return false;
            if (tiles[x][y - 1].prefabs.Count > 0) return false;

            if (tiles[x + 1][y].prefabs.Count > 0) return false;
            if (tiles[x + 1][y + 1].prefabs.Count > 0) return false;
            if (tiles[x + 1][y - 1].prefabs.Count > 0) return false;

            
            if (tiles[x - 1][y].prefabs.Count > 0) return false;
            if (tiles[x - 1][y + 1].prefabs.Count > 0) return false;
            if (tiles[x - 1][y - 1].prefabs.Count > 0) return false;

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
            replacetile(x,y,type,rotation);
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
                    replacetile((x - 4) + i, (y - 4) + j, filltag, 0);
                }
            }
            //Place the centre tile
            replacetile(x, y, type, rotation);
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
                    replacetile((x - 4) + i, (y - 4) + j, filltag, 0);
                }
            }
            //Place the centre tile
            replacetile(x, y, type, rotation);
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
            //Col 1
            replacetile(x - 1, y, filltag, 0);
            replacetile(x - 1, y - 1, filltag, 0);
            replacetile(x - 1, y + 1, filltag, 0);
            //Col 2
            replacetile(x, y, type, rotation);
            replacetile(x, y - 1, filltag, 0);
            replacetile(x, y + 1, filltag, 0);
            //Col 3
            replacetile(x + 1, y, filltag, 0);
            replacetile(x + 1, y - 1, filltag, 0);
            replacetile(x + 1, y + 1, filltag, 0);
            return true;
        }

        public bool placesmalladdontile(int x, int y, string type, int rotation, string filltag)
        {
            addontile(x, y, type, rotation);
            return true;
        }
    }
}
