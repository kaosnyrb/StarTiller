using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Noggog;
using ssf.Generation;
using ssf.Models;
using ssf.POI.Cellgen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Mutagen.Bethesda.Starfield.Package;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ssf.POI
{
    public class POIBuilder
    {
        static Random rng;

        public static ModKey starfieldesm; 

        public static void Setup(BlockLib lib, int seed)
        {
            //Seed the generator
            rng = new Random(seed);
        }



        public static void Generate(int maxsteps, SeedStarfieldSettings settings)
        {
            string pluginname = settings.EspName;
            string datapath = "";


            StarfieldMod myMod;

            using (var env = GameEnvironment.Typical.Builder<IStarfieldMod, IStarfieldModGetter>(GameRelease.Starfield).Build())
            {
                var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();
                datapath = env.DataFolderPath;
                //Find the modkey 
                ModKey newMod = new ModKey(pluginname, ModType.Master);
                myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);
                if (!env.LoadOrder.ModExists(newMod))
                {
                    myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);
                }
                else
                {
                    for (int i = 0; i < env.LoadOrder.Count; i++)
                    {
                        if (env.LoadOrder[i].FileName == pluginname + ".esm")
                        {
                            ModPath modPath = Path.Combine(env.DataFolderPath, env.LoadOrder[i].FileName);
                            myMod = StarfieldMod.CreateFromBinary(modPath, StarfieldRelease.Starfield);

                        }
                    }
                }
                //Location                
                string poiname = NameGenerator.GetRandomPOIName(settings.seed);
                string vowels = "aeiouy ";
                string shortname = poiname.ToLower();
                shortname = new string(shortname.Where(c => !vowels.Contains(c)).ToArray());
                string item = shortname;

                string prefix = myMod.Worldspaces.Count().ToString("000");


                SSFEventLog.EventLogs.Enqueue(poiname);
                SSFEventLog.EventLogs.Enqueue(shortname);
                SSFEventLog.EventLogs.Enqueue(prefix + "wld" + item);
                starfieldesm = env.LoadOrder[0].ModKey;

                IFormLinkNullable<IKeywordGetter> LocTypeDungeon = new FormKey(env.LoadOrder[0].ModKey, 0x000254BC).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocTypeClearable = new FormKey(env.LoadOrder[0].ModKey, 0x00064EDE).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocTypeOE_Keyword = new FormKey(env.LoadOrder[0].ModKey, 0x001A5468).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocEncSpacers_Exclusive = new FormKey(env.LoadOrder[0].ModKey, 0x00283585).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocTypeOverlay = new FormKey(env.LoadOrder[0].ModKey, 0x002CA99D).ToNullableLink<IKeywordGetter>();

                Location location = new Location(myMod)
                {
                    EditorID = prefix + "loc" + item,
                    Name = poiname,
                    Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>(),
                    WorldLocationRadius = 0,
                    ActorFadeMult = 1,
                    TNAM = 0,
                };

                location.Keywords.Add(LocTypeDungeon);
                location.Keywords.Add(LocTypeClearable);
                location.Keywords.Add(LocTypeOE_Keyword);
                location.Keywords.Add(LocEncSpacers_Exclusive);
                location.Keywords.Add(LocTypeOverlay);

                myMod.Locations.Add(location);

                //TopCell
                var topcell = new Cell(myMod)
                {
                    Flags = Cell.Flag.HasWater,
                    Grid = new CellGrid(),
                    WaterHeight = 0,
                    XILS = 1,
                    MajorFlags = Cell.MajorFlag.Persistent,
                    Persistent = new ExtendedList<IPlaced>()
                };

                //Worldspace and terrain

                Worldspace baseworld = myMod.Worldspaces.Where(x => x.EditorID == "stbblock001").First();
                var newworld = myMod.Worldspaces.DuplicateInAsNewRecord(baseworld);
                SurfaceBlock stbblock = myMod.SurfaceBlocks.Where(x => x.EditorID == "OverlayBlockstbblock001").First();
                var newblock = myMod.SurfaceBlocks.DuplicateInAsNewRecord(stbblock);

                string newterrainfile = "Data\\Terrain\\" + prefix + "wld" + item + ".btd";
                try
                {
                    File.Copy(env.DataFolderPath + "\\Terrain\\stbblock001.btd", env.DataFolderPath + "\\Terrain\\" + prefix + "wld" + item + ".btd");
                }
                catch
                {
                    SSFEventLog.EventLogs.Enqueue("Terrain probs exists");
                }
                newblock.ANAM = newterrainfile;
                newblock.EditorID = "OverlayBlock" + prefix + "wld" + item;
                ((WorldSpaceOverlayComponent)newworld.Components[0]).SurfaceBlock = newblock.ToNullableLink<ISurfaceBlockGetter>();

                newworld.EditorID = prefix + "wld" + item;
                newworld.Location = location.ToNullableLink<ILocationGetter>();
                newworld.Name = poiname;
                newworld.TopCell = new Cell(myMod)
                {
                    Flags = Cell.Flag.HasWater,
                    Grid = new CellGrid(),
                    WaterHeight = -200,
                    XILS = 1,
                    MajorFlags = Cell.MajorFlag.Persistent,
                    Persistent = new ExtendedList<IPlaced>()
                };

                //Build map
                int cellid = 0;
                FortCellGen.BuildMap(myMod, settings.seed);
                bool placedboss = false;
                foreach (var sbc in newworld.SubCells)
                {
                    var point = sbc.Items[0].Items[0].Grid.Point;

                    sbc.Items[0].Items[0] = new Cell(myMod)
                    {
                        EditorID = newworld.EditorID + "cell" + cellid++,
                        Grid = new CellGrid() { Point = point },
                        Flags = Cell.Flag.HasWater,
                        XILS = 1,
                        Temporary = new ExtendedList<IPlaced>(),
                        WaterHeight = -200,                        
                    };

                    var packins = FortCellGen.BuildCell(myMod, settings.seed, point);
                    foreach (var pack in packins.Persistant)
                    {
                        newworld.TopCell.Persistent.Add(pack);
                    }
                    foreach (var pack in packins.Temp)
                    {
                        sbc.Items[0].Items[0].Temporary.Add(pack);
                    }

                }
                /*
                //WITH THE ROVER UPDATE DO WE JUST DO THIS BY HAND?
                //Add content node to the branchs
                int id = rng.Next(100);
                //Block
                var pcmcn = new PlanetContentManagerContentNode(myMod)
                {
                    EditorID = "block" + item + id,
                    Content = newworld.ToNullableLink<IPlanetContentTargetGetter>()
                };
                myMod.PlanetContentManagerContentNodes.Add(pcmcn);
                myMod.PlanetContentManagerBranchNodes.Where(x => x.EditorID == "takeovercontent").First().Nodes.Add(pcmcn.ToLinkGetter<IPlanetNodeGetter>());
                //Scan
                var pcmcnscan = new PlanetContentManagerContentNode(myMod)
                {
                    EditorID = "scan" + item + id,
                    Content = newworld.ToNullableLink<IPlanetContentTargetGetter>()
                };
                myMod.PlanetContentManagerContentNodes.Add(pcmcnscan);                
                myMod.PlanetContentManagerBranchNodes.Where(x => x.EditorID == "takeoverscancontent1").First().Nodes.Add(pcmcnscan.ToLinkGetter<IPlanetNodeGetter>());

                //Quest
                var pcmcnquest = new PlanetContentManagerContentNode(myMod)
                {
                    EditorID = "quest" + item + id,
                    Content = newworld.ToNullableLink<IPlanetContentTargetGetter>()
                };
                myMod.PlanetContentManagerContentNodes.Add(pcmcnquest);
                myMod.PlanetContentManagerBranchNodes.Where(x => x.EditorID == "takeoverquestcontent").First().Nodes.Add(pcmcnquest.ToLinkGetter<IPlanetNodeGetter>());
                */
            }
            foreach (var rec in myMod.EnumerateMajorRecords())
            {
                rec.IsCompressed = false;
                //SSFEventLog.EventLogs.Enqueue(rec.FormKey.ToString() + " " + rec.EditorID);
            }
            myMod.WriteToBinary(datapath + "\\" + pluginname + ".esm",new BinaryWriteParameters()
            {
                FormIDUniqueness = FormIDUniquenessOption.Iterate
            });
            SSFEventLog.EventLogs.Enqueue("Export complete!");
        }
    }
}
