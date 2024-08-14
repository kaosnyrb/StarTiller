using Mutagen.Bethesda.Plugins.Binary.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    public class tile
    {
        public string type = "empty";
        public int rotation = 0;
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

        public bool placesmalltile(int x, int y, string type, int rotation)
        {
            if (x > xsize - 2 || x < 1 || y > ysize - 2 || y < 1)
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
            tiles[x-1][y].type = "full";

            tiles[x-1][y-1].rotation = 0;
            tiles[x-1][y-1].type = "full";

            tiles[x - 1][y + 1].rotation = 0;
            tiles[x - 1][y + 1].type = "full";
            //Col 2
            tiles[x][y - 1].rotation = 0;
            tiles[x][y - 1].type = "full";

            tiles[x][y + 1].rotation = 0;
            tiles[x][y + 1].type = "full";

            //Col 3
            tiles[x + 1][y].rotation = 0;
            tiles[x + 1][y].type = "full";

            tiles[x + 1][y - 1].rotation = 0;
            tiles[x + 1][y - 1].type = "full";

            tiles[x + 1][y + 1].rotation = 0;
            tiles[x + 1][y + 1].type = "full";

            return true;
        }
    }
}
