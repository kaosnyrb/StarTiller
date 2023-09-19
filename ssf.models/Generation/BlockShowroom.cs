using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Generation
{
    public class BlockShowroom
    {
        List<Block> Libary;

        List<Block> Output;

        public void Setup(BlockLib lib)
        {
            Libary = lib.blocks.Values.ToList();
            Output = new List<Block>();
        }

        public List<Block> Generate(int maxsteps)
        {
            Output = new List<Block>();
            SSFEventLog.EventLogs.Enqueue("Block Showroom Generation Beginning");

            for(int i = 0; i < Libary.Count();i++)
            {
                Block block = Libary[i].Clone();
                Utils.TranslateBlock(block, new Vector3(0, 5000 * i, 0), 0);
                for (int j = 0;j < 5;j++)
                {
                    var newblock = block.Clone();
                    Utils.TranslateBlock(block, new Vector3(5000 * j, 0, 0), 90 * j);
                    Output.Add(newblock);
                }
            }
            return Output;
        }

    }
}
