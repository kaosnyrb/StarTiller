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
        //TODO

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
            //Find a block that matches a connector
            //with optional dead end finding for closing gaps
            while (BlockDeckPosition < Libary.Count)
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
            }
            return null;
        }

        Block TranslateBlock(Block block, Connector exit)
        {
            Vector3 Pivot = block.blockDetails.startpoint;

            block.blockDetails.BoundingTopLeft = Utils.RotateVectorAroundPivot(Pivot, block.blockDetails.BoundingTopLeft, exit.rotation);
            block.blockDetails.BoundingTopLeft += exit.startpoint;

            block.blockDetails.BoundingBottomRight = Utils.RotateVectorAroundPivot(Pivot, block.blockDetails.BoundingBottomRight, exit.rotation);
            block.blockDetails.BoundingBottomRight += exit.startpoint;
            for (int i = 0; i < block.placedObjects.Count; i++)
            {
                block.placedObjects[i].Placement.translate(Pivot, exit.startpoint, exit.rotation);
            }
            for (int i = 0; i < block.blockDetails.Connectors.Count; i++)
            {
                block.blockDetails.Connectors[i].startpoint = Utils.RotateVectorAroundPivot(Pivot, block.blockDetails.Connectors[i].startpoint, exit.rotation);
                block.blockDetails.Connectors[i].startpoint += exit.startpoint;
                block.blockDetails.Connectors[i].rotation += exit.rotation;
            }
            for (int i = 0; i < block.navmeshs.Count; i++)
            {
                block.navmeshs[i].translate(Pivot, exit.startpoint, exit.rotation);
            }
            return block;
        }

        void PlaceBlock(Block block)
        {
            Output.Add(block);
            for (int i = 0; i < block.blockDetails.Connectors.Count; i++)
            {
                openexits.Add(block.blockDetails.Connectors[i]);
            }
        }

        public int Generate()
        {
            SSFEventLog.EventLogs.Enqueue("Mundus Generation Beginning");
            SSFEventLog.EventLogs.Enqueue("Place the entrance.");
            // 
            int SuccessFullBlocks = 0;
            var start = FindBlockWithJoin("055117:Skyrim.esm", "Entrance", false);
            PlaceBlock(start);
            // While we have exits open.
            int breaker = 30;
            int steps = 0;
            while (openexits.Count > 0 && steps <= breaker)
            {
                int nextexit = rng.Next(openexits.Count);
                // Get the next unplaced exit on the main branch.
                var exit = openexits.ElementAt(nextexit);
                // Select a block that entrance matches the exit
                var nextblock = FindBlockWithJoin(exit.connectorName, "Hall", false);
                //TranslateBlock
                TranslateBlock(nextblock, exit);
                //Collision check
                bool Collision = false;
                foreach(var block in Output)
                {
                    if (Utils.DoBoundingBoxesIntersect(
                        block.blockDetails.BoundingTopLeft, block.blockDetails.BoundingBottomRight,
                        nextblock.blockDetails.BoundingTopLeft, nextblock.blockDetails.BoundingBottomRight))
                    {
                        SSFEventLog.EventLogs.Enqueue("Collision");
                        Collision = true;
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
            return 1;
        }

        public int Export()
        {
            SSFEventLog.EventLogs.Enqueue("Exporting...");
            string pluginname = "bryntest.esp";
            int count = 3000;//3428 works

            string[] files = Directory.GetFiles("Output/Temporary/");
            // Iterate through the files and delete each one
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
                        YamlExporter.WriteObjToYamlFile("Output/Temporary/" + formid + "_" + pluginname + ".yaml", placedobj);
                    }
                }
            }
            SSFEventLog.EventLogs.Enqueue("Export complete!");
            return 1;
        }
    }
}