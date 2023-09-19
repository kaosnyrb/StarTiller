using ssf.IO;
using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;

namespace ssf.Generation
{
    //Based on Mundusform
    public class Mundus
    {
        List<Block> Libary;

        List<Block> Output;

        List<Connector> openexits;
        Random rng;
        int BlockDeckPosition = 0;

        public void Setup(BlockLib lib)
        {
            //Seed the generator
            rng = new Random();
            Libary = lib.blocks.Values.ToList();
            ShuffleDeck();

            Output = new List<Block>();
            openexits = new List<Connector>();
        }

        void ShuffleDeck()
        {
            //Shuffle the blocks so they are randomised a bit
            //We act like it's a deck of card for variation

            Libary = Libary.OrderBy(a => rng.Next()).ToList();
        }

        Block FindBlockWithJoin(string ConectorType, string blocktype, bool deadend)
        {
            int Checkoutcount = 0;

            //Find a block that matches a connector
            //with optional dead end finding for closing gaps
            while (Checkoutcount < Libary.Count * 2)
            {
                Block block = Libary[BlockDeckPosition];
                if (block.blockDetails.startConnector == ConectorType &&
                    block.blockDetails.blocktype == blocktype)
                {
                    //Type and connector match.
                    //Check for deadend

                    if (deadend && block.blockDetails.Connectors.Count == 0)
                    {
                        BlockDeckPosition = 0;
                        ShuffleDeck();
                        return block.Clone();
                    }
                    else if (!deadend)
                    {
                        BlockDeckPosition = 0;
                        ShuffleDeck();
                        return block.Clone();
                    }
                }
                BlockDeckPosition++;
                //We reached the end, shuffle.
                if (BlockDeckPosition >= Libary.Count)
                {
                    BlockDeckPosition = 0;
                    ShuffleDeck();
                }
                Checkoutcount++;
            }
            return null;
        }

        void PlaceBlock(Block block)
        {
            Output.Add(block);
            for (int i = 0; i < block.blockDetails.Connectors.Count; i++)
            {
                openexits.Add(block.blockDetails.Connectors[i]);
            }
        }


        public int Generate(int maxsteps)
        {
            SSFEventLog.EventLogs.Enqueue("Mundus Generation Beginning");
            SSFEventLog.EventLogs.Enqueue("Place the entrance.");
            // 
            int SuccessFullBlocks = 0;
            var start = FindBlockWithJoin("055117:Skyrim.esm", "Entrance", false);
            if (start == null) return 0;
            PlaceBlock(start);
            // While we have exits open.
            int breaker = maxsteps;
            int steps = 0;
            while (openexits.Count > 0 && steps <= breaker)
            {
                int nextexit = rng.Next(openexits.Count);
                // Get the next unplaced exit on the main branch.
                var exit = openexits.ElementAt(nextexit);
                // Select a block that entrance matches the exit
                var nextblock = FindBlockWithJoin(exit.connectorName, "Hall", false);
                if (nextblock == null) return 0;

                //TranslateBlock
                Utils.TranslateBlock(nextblock, exit.startpoint, exit.rotation);
                //Collision check
                bool Collision = false;
                //Bruteforce collision checks
                foreach (var block in Output)
                {
                    foreach (var exisitingobj in block.placedObjects)
                    {
                        foreach (var newobj in nextblock.placedObjects)
                        {
                            if (!exisitingobj.EditorID.Contains("ExitBlock"))
                            {
                                Vector3 dist = Utils.ConvertStringToVector3(exisitingobj.Placement.Position) - Utils.ConvertStringToVector3(newobj.Placement.Position);
                                if (dist.Length() < 64)
                                {
                                    Collision = true;
                                }
                            }
                        }
                    }
                }
                //Place
                if (!Collision)
                {
                    openexits.RemoveAt(nextexit);
                    PlaceBlock(nextblock);
                    SSFEventLog.EventLogs.Enqueue("Placed Block: " + nextblock.path);
                    SuccessFullBlocks++;
                }
                steps++;
            }
            SSFEventLog.EventLogs.Enqueue("Total Blocks: " + SuccessFullBlocks);
            return SuccessFullBlocks;
        }

        public int Export()
        {
            SSFEventLog.EventLogs.Enqueue("Exporting...");
            string pluginname = "bryntest.esp";
            int count = 3000;//3428 works

            // Iterate through the files and delete each one
            string[] files = Directory.GetFiles("Output/Temporary/");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            // Iterate through the files and delete each one
            files = Directory.GetFiles("Output/NavigationMeshes/");
            foreach (string file in files)
            {
                File.Delete(file);
            }

            foreach (var outblock in Output)
            {
                //SSFEventLog.EventLogs.Enqueue("Exporting block " + outblock.path);
                foreach (var placedobj in outblock.placedObjects)
                {
                    if (!placedobj.EditorID.Contains("ExitBlock"))
                    {
                        placedobj.EditorID = "";//We clear this to stop collisions.
                        placedobj.SkyrimMajorRecordFlags = new List<int>();
                        count++;
                        string formid = count.ToString("X6");
                        placedobj.FormKey = formid + ":" + pluginname;
                        if (placedobj.Placement.Rotation.Contains("E"))
                        {
                            SSFEventLog.EventLogs.Enqueue("Odd Rotation: " + placedobj.Placement.Rotation);
                        }
                        YamlExporter.WriteObjToYamlFile("Output/Temporary/" + formid + "_" + pluginname + ".yaml", placedobj);
                    }
                }
                foreach(var navmesh in outblock.navmeshs)
                {
                    count++;
                    string formid = count.ToString("X6");
                    navmesh.VersionControl = 12079;
                    navmesh.FormKey = formid + ":" + pluginname;
                    navmesh.Data.Parent.Parent = "000D62:bryntest.esp";
                    YamlExporter.WriteObjToYamlFile("Output/NavigationMeshes/" + formid + "_" + pluginname + ".yaml", navmesh);
                }
            }
            SSFEventLog.EventLogs.Enqueue("Export complete!");
            return 1;
        }
    }
}