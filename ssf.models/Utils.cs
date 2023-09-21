using ssf.IO;
using ssf.Manipulators;
using ssf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Reflection.Metadata.BlobBuilder;

namespace ssf
{
    public class Utils
    {
        public static Dictionary<string, Block> LoadBlockLib(string folderPath)
        {
            Dictionary<string, Block> blocks = new Dictionary<string, Block>();
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

                        if (block.Contains("Hall"))
                        {
                            newBlock.blockDetails.blocktype = "Hall";
                        }
                        if (block.Contains("Entrance"))
                        {
                            newBlock.blockDetails.blocktype = "Entrance";
                        }
                        if (block.Contains("Room"))
                        {
                            newBlock.blockDetails.blocktype = "Room";
                        }

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
                                    ZeroingX = ConvertStringToVector3(obj.Placement.Position).X;
                                    ZeroingY = ConvertStringToVector3(obj.Placement.Position).Y;
                                    ZeroingZ = ConvertStringToVector3(obj.Placement.Position).Z;
                                    //SSFEventLog.EventLogs.Enqueue("Zeroing at: " + ZeroingZ);
                                }
                            }
                        }


                        foreach (string placedobj in files)
                        {
                            var result = File.ReadAllText(placedobj);
                            PlacedObject obj = YamlImporter.getObjectFromYaml<PlacedObject>(result);

                            // Dump anything that doesn't rotate well.
                            /*
                            if (ConvertStringToVector3(obj.Placement.Rotation).X != 0 ||
                                ConvertStringToVector3(obj.Placement.Rotation).Y != 0)
                            {
                                continue;
                            }*/
                            //sort the height
                            if (ZeroingZ != 0)
                            {
                                var heighfixpos = ConvertStringToVector3(obj.Placement.Position);
                                heighfixpos.X -= ZeroingX;
                                heighfixpos.Y -= ZeroingY;
                                heighfixpos.Z -= ZeroingZ;
                                obj.Placement.Position = ConvertVector3ToString(heighfixpos);
                            }

                            //Clean the rotations
                            var rotationLimiter = 0.01f; //0.1 is about 5 degrees
                            var resultRotation = ConvertStringToVector3(obj.Placement.Rotation);
                            if (resultRotation.X < rotationLimiter && resultRotation.X > -rotationLimiter) resultRotation.X = 0;
                            if (resultRotation.Y < rotationLimiter && resultRotation.Y > -rotationLimiter) resultRotation.Y = 0;
                            if (resultRotation.Z < rotationLimiter && resultRotation.Z > -rotationLimiter) resultRotation.Z = 0;
                            obj.Placement.Rotation = ConvertVector3ToString(resultRotation);

                            //Testing using info we've exported to fill in other stuff we need.
                            if (obj.EditorID == null)
                            {
                                obj.EditorID = "";
                            }
                            if (obj.EditorID.Contains("StartBlock"))
                            {
                                newBlock.blockDetails.startpoint = ConvertStringToVector3(obj.Placement.Position);
                                newBlock.blockDetails.startRotation = ConvertStringToVector3(obj.Placement.Rotation);
                                newBlock.blockDetails.startConnector = obj.Base;
                                //newBlock.blockDetails.blocktype = "?"; //Maybe path based?
                            }
                            if (obj.EditorID.Contains("ExitBlock"))
                            {
                                Connector newexit = new Connector()
                                {
                                    connectorName = obj.Base,
                                    startpoint = ConvertStringToVector3(obj.Placement.Position),
                                    rotation = ToRadians(ConvertStringToVector3(obj.Placement.Rotation).Z)
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
                            for (int i = 0; i < mesh.Data.Vertices.Count(); i++)
                            {
                                var pos = ConvertStringToVector3(mesh.Data.Vertices[i]);
                                pos -= new Vector3(ZeroingX, ZeroingY, ZeroingZ);
                                mesh.Data.Vertices[i] = ConvertVector3ToString(pos);
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
                            TranslateBlock(newBlock, newBlock.blockDetails.startpoint, -rotationneeded);
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
            SSFEventLog.EventLogs.Enqueue("Block lib loaded, Block count :" + blocks.Count.ToString());
            return blocks;
        }


        public static Vector3 ConvertStringToVector3(string input)
        {
            if (input == null) return new Vector3(0, 0, 0);

            // Split the input string by commas.
            string[] components = input.Split(',');

            // Check if there are exactly three components (x, y, and z).
            if (components.Length != 3)
            {
                return Vector3.Zero; // Return a default Vector3 if the input is invalid.
            }

            // Parse the components as floats.
            float x, y, z;
            if (float.TryParse(components[0], out x) &&
                float.TryParse(components[1], out y) &&
                float.TryParse(components[2], out z))
            {
                // Create and return the Vector3.
                return new Vector3(x, y, z);
            }
            else
            {
                return Vector3.Zero; // Return a default Vector3 if parsing fails.
            }
        }

        public static string ConvertVector3ToString(Vector3 vector)
        {
            // Use string interpolation to format the Vector3 as a string.
            string result = $"{vector.X}, {vector.Y}, {vector.Z}";
            return result;
        }

        public static Vector3 RotateVectorAroundPivot(Vector3 pivot, Vector3 p, double angle)
        {
            angle = angle * (Math.PI / 180);

            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            // translate point back to origin:
            p.X -= pivot.X;
            p.Y -= pivot.Y;

            // rotate point
            double xnew = p.X * c - p.Y * s;
            double ynew = p.X * s + p.Y * c;

            if (xnew < 0.01 && xnew > -0.01)
            {
                xnew = 0;
            }
            if (ynew < 0.01 && ynew > -0.01)
            {
                ynew = 0;
            }

            // translate point back:
            p.X = (float)(xnew + pivot.X);
            p.Y = (float)(ynew + pivot.Y);

            return p;
        }

        public static Block TranslateBlock(Block block, Vector3 Pos, float Rotation)
        {
            //SSFEventLog.EventLogs.Enqueue("Translating " + block.path + " by " + Pos + " and " + Rotation);
            Vector3 Pivot = block.blockDetails.startpoint;
            for (int i = 0; i < block.placedObjects.Count; i++)
            {
                //Convert string to vector3
                var pos = ConvertStringToVector3(block.placedObjects[i].Placement.Position);
                //Rotate around pivot 
                if (Rotation != 0)
                {
                    pos = RotateVectorAroundPivot(Pivot, pos, Rotation);
                }
                //Apply translation
                pos += Pos;
                //convert back to string (needed for export)
                block.placedObjects[i].Placement.Position = ConvertVector3ToString(pos);
                //Apply Rotation
                if (Rotation != 0)
                {
                    var rot = ConvertStringToVector3(block.placedObjects[i].Placement.Rotation);

                    var resultRotation = RotationUtils.rotateAroundZ(rot, ToRadians(-Rotation));
                    var rotationLimiter = 0.001f;
                    if (resultRotation.X < rotationLimiter && resultRotation.X > -rotationLimiter) resultRotation.X = 0;
                    if (resultRotation.Y < rotationLimiter && resultRotation.Y > -rotationLimiter) resultRotation.Y = 0;
                    if (resultRotation.Z < rotationLimiter && resultRotation.Z > -rotationLimiter) resultRotation.Z = 0;
                    block.placedObjects[i].Placement.Rotation = ConvertVector3ToString(resultRotation);
                }
            }
            for (int i = 0; i < block.blockDetails.Connectors.Count; i++)
            {
                block.blockDetails.Connectors[i].startpoint = RotateVectorAroundPivot(Pivot, block.blockDetails.Connectors[i].startpoint, Rotation);
                block.blockDetails.Connectors[i].startpoint += Pos;
                block.blockDetails.Connectors[i].rotation += Rotation;
            }
            for (int i = 0; i < block.navmeshs.Count; i++)
            {
                for (int j = 0; j < block.navmeshs[i].Data.Vertices.Length; j++)
                {
                    //Convert string to vector3
                    var pos = ConvertStringToVector3(block.navmeshs[i].Data.Vertices[j]);

                    //Rotate around pivot 
                    pos = RotateVectorAroundPivot(Pivot, pos, Rotation);

                    //Apply translation
                    pos += Pos;

                    //convert back to string (needed for export)
                    block.navmeshs[i].Data.Vertices[j] = ConvertVector3ToString(pos);
                }
            }
            return block;
        }

        public static float ToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180));
        }
        public static float ToDegrees(float rads)
        {
            return (float)(rads * (180 / Math.PI));
        }
    }
}
