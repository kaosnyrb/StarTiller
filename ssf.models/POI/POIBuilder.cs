using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Noggog;
using ssf.Generation;
using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ssf.POI
{
    public class POIBuilder
    {
        static Random rng;

        public static void Setup(BlockLib lib, int seed)
        {
            //Seed the generator
            rng = new Random(seed);
        }

        public static void Generate(int maxsteps, SeedStarfieldSettings settings)
        {
            string pluginname = settings.EspName;
            string datapath = "";

            string prefix = "1to";

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

                SSFEventLog.EventLogs.Enqueue(poiname);
                SSFEventLog.EventLogs.Enqueue(shortname);
                SSFEventLog.EventLogs.Enqueue(prefix + "wld" + item);
                
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
                var baseworld = myMod.Worldspaces[new FormKey(myMod.ModKey, 0x00000CEB)];
                var newworld = myMod.Worldspaces.DuplicateInAsNewRecord(baseworld);               
                var newblock = myMod.SurfaceBlocks.DuplicateInAsNewRecord(myMod.SurfaceBlocks[new FormKey(myMod.ModKey, 0x00000CEC)]);
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
                int ia = 0;
                foreach(var sbc in newworld.SubCells)
                {
                    var point = sbc.Items[0].Items[0].Grid.Point;

                    sbc.Items[0].Items[0] = new Cell(myMod)
                    {
                        EditorID = prefix+newworld.EditorID + "cell" + ia++,
                        Grid = new CellGrid() { Point = point },
                        Flags = Cell.Flag.HasWater,
                        XILS = 1,
                        Temporary = new ExtendedList<IPlaced>(),
                        WaterHeight = -200,                        
                    };

                    //Add blocks
                    //Cell sizes
                    // 10,10,-10
                    // 90,90,-10
                    //Block offset matters
                    //Block is 16
                    IFormLinkNullable<IPlaceableObjectGetter> to_pkn_base = new FormKey(myMod.ModKey, 0x000014BE).ToNullableLink<IPlaceableObjectGetter>();
                    for(int x = 0; x < 5; x++)
                    {
                        for(int y = 0; y < 5; y++) 
                        {
                            var inewblock = new Mutagen.Bethesda.Starfield.PlacedObject(myMod)
                            {
                                Base = to_pkn_base,
                                Position = new P3Float((point.X * 100) + (10 + (16 * x)), (point.Y * 100) + 10 + (16 * y), -10)

                            };
                            sbc.Items[0].Items[0].Temporary.Add(inewblock);
                        }
                    }
                }
                //Add content node to the branch
                Random rand = new Random();
                var pcmcn = new PlanetContentManagerContentNode(myMod)
                {
                    EditorID = item + rand.Next(100),
                    Content = newworld.ToNullableLink<IPlanetContentTargetGetter>()
                };
                myMod.PlanetContentManagerContentNodes.Add(pcmcn);
                myMod.PlanetContentManagerBranchNodes[new FormKey(myMod.ModKey, 0x00000F66)].Nodes.Add(pcmcn.ToLinkGetter<IPlanetNodeGetter>());

            }
            foreach (var rec in myMod.EnumerateMajorRecords())
            {
                rec.IsCompressed = false;
            }
            myMod.WriteToBinary(datapath + "\\" + pluginname + ".esm");
            SSFEventLog.EventLogs.Enqueue("Export complete!");
        }
    }
}
