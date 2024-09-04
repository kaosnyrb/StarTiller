using Mutagen.Bethesda.Starfield;
using Noggog;
using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.POI.Cellgen
{
    public interface StartillerGeneratorInstance
    {
        public void BuildPkns(StarfieldMod myMod);
        public void BuildMap(StarfieldMod myMod, int seed);

        public CellGenerationResult BuildCell(StarfieldMod myMod, int seed, P2Int cellPos);
    }
}
