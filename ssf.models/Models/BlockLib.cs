using ssf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    //This holds all the block information from the content folder
    public class BlockLib
    {
        public static BlockLib Instance { get; set; }

        public Dictionary<string, Block> blocks;

        public bool LoadBlockLib(string folderPath)
        {
            blocks = new Dictionary<string, Block>();
            string[] subfolders = Directory.GetDirectories(folderPath);
            foreach (string blocktype in subfolders)
            {
                //Types
                string[] blockfolders = Directory.GetDirectories(blocktype);
                foreach (string block in blockfolders)
                {
                    Block newBlock = new Block()
                    {
                        path = block,
                        placedObjects = new List<PlacedObject>(),
                        navmeshs = new List<Navmesh>(),
                    };
                    string[] files = Directory.GetFiles(block + "//Temporary//");
                    foreach (string placedobj in files)
                    {
                        var result = File.ReadAllText(placedobj);
                        PlacedObject obj = YamlImporter.getObjectFromYaml<PlacedObject>(result);
                        newBlock.placedObjects.Add(obj);
                    }
                    string[] navmeshes = Directory.GetFiles(block + "//NavigationMeshes//");
                    foreach (string navmesh in navmeshes)
                    {
                        var result = File.ReadAllText(navmesh);
                        Navmesh mesh = YamlImporter.getObjectFromYaml<Navmesh>(result);
                        newBlock.navmeshs.Add(mesh);
                    }
                    blocks.Add(block, newBlock);
                }
            }
            return true;
        }
    }

    public class Block
    {
        public string path;
        public List<PlacedObject> placedObjects;
        public List<Navmesh> navmeshs;

    }
}
