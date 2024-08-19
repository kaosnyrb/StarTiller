using Mutagen.Bethesda.Starfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Models
{
    //Returns the build results of a cell
    //Persistant items exist at a greater distance, and are more expensive.
    //We only want to have things that are nessicary here.
    public class CellGenerationResult
    {
        public List<IPlaced> Persistant;
        public List<IPlaced> Temp;

    }
}
