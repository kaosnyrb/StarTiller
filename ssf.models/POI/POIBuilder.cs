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

namespace ssf.POI
{
    public class POIBuilder
    {
        static Random rng;

        public static ModKey starfieldesm;
        public static StartillerGeneratorInstance MapGenerator;

        public static void Setup(StartillerGeneratorInstance generator, int seed)
        {
            //Seed the generator
            rng = new Random(seed);
            MapGenerator = generator;
        }

        public static void Generate(int maxsteps, SeedStarfieldSettings settings)
        {
            string pluginname = settings.EspName;
            string datapath = "";
            StarfieldMod myMod;
            using (var env = GameEnvironment.Typical.Builder<IStarfieldMod, IStarfieldModGetter>(GameRelease.Starfield).Build())
            {

                //Load the ESM
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


                //Build the Location Form
                string poiname = NameGenerator.GetRandomPOIName(settings.seed);
                string vowels = "aeiouy ";
                string shortname = poiname.ToLower();
                shortname = new string(shortname.Where(c => !vowels.Contains(c)).ToArray());
                string prefix = myMod.Worldspaces.Count().ToString("000");

                SSFEventLog.EventLogs.Enqueue("Building new POI : " + poiname);
                SSFEventLog.EventLogs.Enqueue(prefix + "wld" + shortname);
                starfieldesm = env.LoadOrder[0].ModKey;//Store the Starfield ESM for later.

                //Location Keywords, Important for bounties/Block Overlay stuff
                IFormLinkNullable<IKeywordGetter> LocTypeDungeon = new FormKey(starfieldesm, 0x000254BC).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocTypeClearable = new FormKey(starfieldesm, 0x00064EDE).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocTypeOE_Keyword = new FormKey(starfieldesm, 0x001A5468).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocEncSpacers_Exclusive = new FormKey(starfieldesm, 0x00283585).ToNullableLink<IKeywordGetter>();
                IFormLinkNullable<IKeywordGetter> LocTypeOverlay = new FormKey(starfieldesm, 0x002CA99D).ToNullableLink<IKeywordGetter>();

                Location location = new Location(myMod)
                {
                    EditorID = prefix + "loc" + shortname,
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

                //Topcell is the Persistant cell for the Worldspace
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
                //To parameterise:
                string realworldspaceeditorid = "stbblock001";
                string realSurfaceBlockEditorId = "OverlayBlockstbblock001";

                //We clone the existing Worldspace then build ontop of it.
                Worldspace baseworld = myMod.Worldspaces.Where(x => x.EditorID == realworldspaceeditorid).First();
                var newworld = myMod.Worldspaces.DuplicateInAsNewRecord(baseworld);
                SurfaceBlock stbblock = myMod.SurfaceBlocks.Where(x => x.EditorID == realSurfaceBlockEditorId).First();
                var newblock = myMod.SurfaceBlocks.DuplicateInAsNewRecord(stbblock);

                //Terrain files must match worldspace names percisely.
                string newterrainfile = "Data\\Terrain\\" + prefix + "wld" + shortname + ".btd";
                try
                {
                    File.Copy(env.DataFolderPath + "\\Terrain\\stbblock001.btd", env.DataFolderPath + "\\Terrain\\" + prefix + "wld" + shortname + ".btd");
                }
                catch
                {
                    SSFEventLog.EventLogs.Enqueue("Terrain probs exists");
                }
                newblock.ANAM = newterrainfile;
                newblock.EditorID = "OverlayBlock" + prefix + "wld" + shortname;
                ((WorldSpaceOverlayComponent)newworld.Components[0]).SurfaceBlock = newblock.ToNullableLink<ISurfaceBlockGetter>();
                newworld.EditorID = prefix + "wld" + shortname;
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
                //Now we have a worldspace we have to fill each of the Cells in it.
                int cellid = 0;
                MapGenerator.BuildMap(myMod, settings.seed);
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

                    // Generate each cell in the worldspace and add the Persistant and Temporary items.
                    var packins = MapGenerator.BuildCell(myMod, settings.seed, point);
                    foreach (var pack in packins.Persistant)
                    {
                        newworld.TopCell.Persistent.Add(pack);
                    }
                    foreach (var pack in packins.Temp)
                    {
                        sbc.Items[0].Items[0].Temporary.Add(pack);
                    }

                }                
            }
            //This is needed for now as Mutagen can't handle compression. Believe this is fixed in the CK.
            foreach (var rec in myMod.EnumerateMajorRecords())
            {
                rec.IsCompressed = false;
                //SSFEventLog.EventLogs.Enqueue(rec.FormKey.ToString() + " " + rec.EditorID);//Useful for listing Formids
            }
            myMod.WriteToBinary(datapath + "\\" + pluginname + ".esm",new BinaryWriteParameters()
            {
                FormIDUniqueness = FormIDUniquenessOption.Iterate
            });
            SSFEventLog.EventLogs.Enqueue("Export complete!");
        }
    }
}
