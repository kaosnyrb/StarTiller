﻿using ssf.IO;
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
                        Block newBlock = new Block()
                        {
                            blockDetails = new BlockDetails(),
                            path = block,
                            placedObjects = new List<PlacedObject>(),
                            navmeshs = new List<Navmesh>(),
                        };
                        newBlock.blockDetails.Connectors = new List<Connector>();
                        string[] files = Directory.GetFiles(block + "//Temporary//");

                        //Find the floor
                        //This prob could be optimised but it's only on startup
                        float ZeroingX = 0;
                        float ZeroingY = 0;
                        float ZeroingZ = 0;
                        foreach (string placedobj in files)
                        {
                            var result = File.ReadAllText(placedobj);
                            PlacedObject obj = YamlImporter.getObjectFromYaml<PlacedObject>(result);
                            if (obj.EditorID != null)
                            {
                                if (obj.EditorID.Contains("StartBlock"))
                                {
                                    //We move everything so the start block is at 0 height
                                    ZeroingX = Utils.ConvertStringToVector3(obj.Placement.Position).X;
                                    ZeroingY = Utils.ConvertStringToVector3(obj.Placement.Position).Y;
                                    ZeroingZ = Utils.ConvertStringToVector3(obj.Placement.Position).Z;
                                    //SSFEventLog.EventLogs.Enqueue("Zeroing at: " + ZeroingZ);
                                }
                            }
                        }


                        foreach (string placedobj in files)
                        {
                            var result = File.ReadAllText(placedobj);
                            PlacedObject obj = YamlImporter.getObjectFromYaml<PlacedObject>(result);
                            if (placedobj.Contains("Hall"))
                            {
                                newBlock.blockDetails.blocktype = "Hall";
                            }
                            if (placedobj.Contains("Entrance"))
                            {
                                newBlock.blockDetails.blocktype = "Entrance";
                            }
                            if (placedobj.Contains("Room"))
                            {
                                newBlock.blockDetails.blocktype = "Room";
                            }
                            /*
                            if (Utils.ConvertStringToVector3(obj.Placement.Rotation).X != 0 ||
                                Utils.ConvertStringToVector3(obj.Placement.Rotation).Y != 0)
                            {
                                continue;
                            }*/
                            //sort the height
                            if (ZeroingZ != 0)
                            {
                                var heighfixpos = Utils.ConvertStringToVector3(obj.Placement.Position);
                                heighfixpos.X -= ZeroingX;
                                heighfixpos.Y -= ZeroingY;
                                heighfixpos.Z -= ZeroingZ;
                                obj.Placement.Position = Utils.ConvertVector3ToString(heighfixpos);
                            }

                            //Testing using info we've exported to fill in other stuff we need.
                            if (obj.EditorID == null)
                            {
                                obj.EditorID = "";
                            }
                            if (obj.EditorID.Contains("StartBlock"))
                            {
                                newBlock.blockDetails.startpoint = Utils.ConvertStringToVector3(obj.Placement.Position);
                                newBlock.blockDetails.startRotation = Utils.ConvertStringToVector3(obj.Placement.Rotation);
                                newBlock.blockDetails.startConnector = obj.Base;
                                //newBlock.blockDetails.blocktype = "?"; //Maybe path based?
                            }
                            if (obj.EditorID.Contains("ExitBlock"))
                            {
                                Connector newexit = new Connector()
                                {
                                    connectorName = obj.Base,
                                    startpoint = Utils.ConvertStringToVector3(obj.Placement.Position),
                                    rotation = (float)(Utils.ConvertStringToVector3(obj.Placement.Rotation).Z * 57.2958)//Rads to degrees
                                };
                                newBlock.blockDetails.Connectors.Add(newexit);
                            }
                            newBlock.placedObjects.Add(obj);
                        }

                        string[] navmeshes = Directory.GetFiles(block + "//NavigationMeshes//");
                        foreach (string navmesh in navmeshes)
                        {
                            var result = File.ReadAllText(navmesh);
                            Navmesh mesh = YamlImporter.getObjectFromYaml<Navmesh>(result);
                            //Zero the navmesh.
                            for(int i = 0; i < mesh.Data.Vertices.Count(); i++)
                            {
                                var pos = Utils.ConvertStringToVector3(mesh.Data.Vertices[i]);
                                pos -= new Vector3(ZeroingX, ZeroingY, ZeroingZ);
                                mesh.Data.Vertices[i] = Utils.ConvertVector3ToString(pos);
                            }
                            newBlock.navmeshs.Add(mesh);
                        }

                        //Unrotate the block.
                        //If the start or end blocks are the wrong way round things get wierd.
                        // TODO stop blocks being the wrong way round
                        if (newBlock.blockDetails.startRotation.Z != 0)
                        {
                            float rotationneeded = (float)(newBlock.blockDetails.startRotation.Z * 57.2958);
                            SSFEventLog.EventLogs.Enqueue("Rotating Block" + newBlock.path + " by " + -rotationneeded);
                            Utils.TranslateBlock(newBlock, newBlock.blockDetails.startpoint, -rotationneeded);
                            newBlock.blockDetails.startRotation = new Vector3(0, 0, 0);
                        }

                        blocks.Add(block, newBlock);
                    }
                    catch (Exception ex)
                    {
                        SSFEventLog.EventLogs.Enqueue(ex.Message);
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