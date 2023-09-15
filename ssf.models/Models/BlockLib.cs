using ssf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
                    try
                    {
                        var blockDetails = File.ReadAllText(block + "//blockDetails.yaml");
                        BlockDetails blockDetailsobj = YamlImporter.getObjectFromYaml<BlockDetails>(blockDetails);
                        Block newBlock = new Block()
                        {
                            blockDetails = blockDetailsobj,
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
                    catch (Exception ex)
                    {

                    }
                }
            }
            SSFEventLog.EventLogs.Enqueue("Block lib loaded, Block count :" + BlockLib.Instance.blocks.Count.ToString());
            return true;
        }
    }

    public class Block
    {
        public BlockDetails blockDetails { get; set; }
        public string path;
        public List<PlacedObject> placedObjects;
        public List<Navmesh> navmeshs;

        public Block Clone()
        {
            //Eh if it works.
            var clone = YamlImporter.getObjectFromYaml<Block>(YamlExporter.BuildYaml(this));
            return clone;
        }

        public Block Translate(Vector3 position)
        {
            //TODO
            blockDetails.startpoint = position;
            foreach (var connector in blockDetails.Connectors)
            {
                connector.startpoint += position;
            }
            foreach (var obj in placedObjects) {
                obj.Placement.translate(new Vector3(0,0,0), position, 0);
            }
            return this;
        }
    }
}