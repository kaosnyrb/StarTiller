using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.Generation
{
    public class NameGenerator
    {
        public static List<string> adj = new List<string>
        {
            "Forsaken",
            "Deserted",
            "Neglected",
            "Discarded",
            "Forgotten",
            "Abandoned",
            "Relinquished",
            "Vacant",
            "Desolate",
            "Unoccupied",
            "Derelict",
            "Despoiled",
            "Isolated",
            "Renounced",
            "Surrendered",
            "Disowned",
            "Disregarded",
            "Unclaimed",
            "Ignored",
            "Forsook",
            "Vacated",
            "Uninhabited",
            "Unprotected",
            "Destitute",
            "Forgotten",
            "Dilapidated",
            "Ramshackle",
            "Decrepit",
            "Desolate",
            "Decayed",
        };

        public static List<string> noun = new List<string>
        {
            "Warehouse",
            "Storehouse",
            "Storage",
            "Station",
            "Terminal",
            "Store",
            "Repository",
            "Stockroom",
            "Storage Facility",
            "Supply Center",
            "Distribution Center",
            "Freight Station",
            "Goods Yard",
            "Stockpile",
            "Storage Depot",
            "Transit Point",
            "Holding Area",
            "Shipping Center",
            "Supply Depot",
            "Loading Dock",
            "Processing Plant",
            "Manufacturing Plant",
            "Factory",
            "Plant",
            "Mill",
            "Purification Plant",
            "Smelting Plant",
            "Extraction Plant",
            "Industrial Facility",
            "Processing Facility",
            "Distillery",
            "Chemical Plant",
            "Production Facility",
            "Refining Plant",
            "Treatment Plant",
            "Base",
            "Garrison",
            "Fort",
            "Camp",
            "Outpost",
            "Barracks",
            "Station",
            "Encampment",
            "Installation",
            "Stronghold",
            "Command Center",
            "Military Base",
            "Post",
            "Cantonment",
            "Headquarters",
            "Defense Post",
            "Army Post",
            "Operational Base",
            "Staging Area",
            "Military Installation"
        };


        public static string GetRandomPOIName(int seed)
        {
            string result = "";
            Random rand = new Random(seed);

            result = adj[rand.Next(adj.Count - 1)] + " " + noun[rand.Next(noun.Count - 1)];

            return result;
        }
    }
}
