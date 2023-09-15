using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Generation
{
	//Based on Mundusform
	public class Mundus
	{
		List<Block> Libary;
		int BlockDeckPosition = 0;
        //TODO

		void Setup(BlockLib lib)
		{
			Libary = lib.blocks.Values.ToList();
			ShuffleDeck();
        }

        void ShuffleDeck()
        {
            //Shuffle the blocks so they are randomised a bit
            //We act like it's a deck of card for variation
            Random rng = new Random();
			var shuffledcards = Libary.OrderBy(a => rng.Next()).ToList();
        }

        //Note that this is C++, need to port.
        /*
 		Block FindBlockWithJoin(const char* ConectorType, const char* blocktype, bool deadend)
		{
            //Find a block that matches a connector
            //with optional dead end finding for closing gaps
		}
	    
        Block TranslateBlock(Block block, Tile exit)
	    {
			// Move box to the connector exit
			block.boundingbox.position.x += exit.x;
			block.boundingbox.position.y += exit.y;
			for (int i = 0; i < block.reflist.length; i++)
			{
				block.reflist.data[i].pos.x += exit.x;
				block.reflist.data[i].pos.y += exit.y;
				block.reflist.data[i].pos.z += exit.z;
			}
			for (int i = 0; i < block.exitslist.length; i++)
			{
				block.exitslist.data[i].x += exit.x;
				block.exitslist.data[i].y += exit.y;
				block.exitslist.data[i].z += exit.z;
				block.exitslist.data[i].bearing += exit.bearing;
			}
			for (int i = 0; i < block.navlist.length; i++)
			{
				block.navlist.data[i].x += exit.x;
				block.navlist.data[i].y += exit.y;
				block.navlist.data[i].z += exit.z;
			}
			return block;
        }

        TileList PlaceBlock(Block block)
	    {
			//Render the block into the output
			_MESSAGE("Place the block");
			boundingboxes.AddItem(block.boundingbox);
			for (int i = 0; i < block.reflist.length; i++)
			{
				formlist.AddItem(block.reflist.data[i]);
			}
			_MESSAGE("Update the navmesh");
			for (int i = 0; i < block.navlist.length; i++)
			{
				MarkTile(block.navlist.data[i].x, block.navlist.data[i].y, block.navlist.data[i].z, block.navlist.data[i].quadsize);
			}
			TileList newexits = TileList();
			for (int i = 0; i < block.exitslist.length; i++)
			{
				newexits.AddItem(block.exitslist.data[i]);
			}
			return newexits;
        }

    	void BuildRift(VMClassRegistry* registry, TESObjectREFR* Target, TESObjectCELL* cell, TESWorldSpace* worldspace)
	    {
			//Export
        }


		int Generate()
		{
			_MESSAGE("Place the enterance.");
			_MESSAGE("While we have exits open");
			_MESSAGE("Get the next unplaced exit on the main branch.");
			_MESSAGE("Select a block that entrance matches the exit");
			//FindBlockWithJoin
			//RotateAroundPivot
			//TranslateBlock
			//BlockFitsExit
		}

         */
    }
}
