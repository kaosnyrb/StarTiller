using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf
{
    public class SeedStarfieldSettings
    {
        public string ContentPath { get; set; }
        public int GenLength { get; set; }
        public int MinBlocks { get; set; }

        public int seed { get;set; }

        public string cellname { get; set; }
        public string EspName { get; set; }
        public string ExportPath { get; set; }
        public int FormIdOffset { get; set; }
        public string DataFolder {  get; set; }
        public string SpriggitCli {  get; set; }
        public string GitModPath {  get; set; }

    }
}
