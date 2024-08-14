using ssf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ssf.Models
{
    //This holds all the block information from the content folder
    public class BlockLib
    {
        public static BlockLib Instance { get; set; }

        public Dictionary<string, Block> blocks;        
    }

    public class Block
    {
        public BlockDetails blockDetails { get; set; }
        public string path;
        //public List<PlacedObject> placedObjects;
        //public List<Navmesh> navmeshs;

        public Block Clone()
        {
            //Eh if it works.
            var clone = YamlImporter.getObjectFromYaml<Block>(YamlExporter.BuildYaml(this));
            return clone;
        }
    }
}