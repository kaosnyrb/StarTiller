using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;
using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            string prefix = settings.cellname;
            string item = "suffix";
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

                //What we need to build:
                /*
                //Packin Cell
                Console.WriteLine("Building Record : " + prefix + "_cell_" + item);
                var newCell = new Cell(myMod)
                {
                    EditorID = settings.cellname,
                    Temporary = new ExtendedList<IPlaced>(),
                    Flags = Cell.Flag.IsInteriorCell,
                    Lighting = new CellLighting()
                    {
                        DirectionalFade = 1,
                        FogPower = 1,
                        FogMax = 1,
                        NearHeightRange = 10000,
                        Unknown1 = 1951,
                        Unknown2 = 3,
                    },
                };
                IFormLink<IPlaceableObjectGetter> PrefabPackinPivotDummy = new FormKey(env.LoadOrder[0].ModKey, 0x0003F808).ToLink<IPlaceableObjectGetter>();
                newCell.Temporary.Add(new Mutagen.Bethesda.Starfield.PlacedObject(myMod)
                {
                    Base = PrefabPackinPivotDummy,
                    Position = new P3Float(0, 0, 0),
                    Rotation = new P3Float(0, 0, 0)
                });
                var key = newCell.FormKey.ID;
                var stringkey = key.ToString();
                var cellblockNumber = int.Parse(stringkey.Substring(stringkey.Length - 1));
                var subBlockNumber = int.Parse(stringkey.Substring(stringkey.Length - 2, 1));

                SSFEventLog.EventLogs.Enqueue("Created Cell :" + key.ToString("x"));
                //Try and use existing cellblocks and subblocks first.
                CellBlock cellblock = null;
                bool newCellBlock = false;
                for (int i = 0; i < myMod.Cells.Count; i++)
                {
                    if (myMod.Cells[i].BlockNumber == cellblockNumber)
                    {
                        cellblock = myMod.Cells[i];
                        break;
                    }
                }
                if (cellblock == null)
                {
                    cellblock = new CellBlock
                    {
                        BlockNumber = cellblockNumber,
                        GroupType = GroupTypeEnum.InteriorCellBlock,
                        SubBlocks = new ExtendedList<CellSubBlock>()
                    };
                    newCellBlock = true;
                }

                bool addSubblock = true;
                for (int i = 0; i < cellblock.SubBlocks.Count; i++)
                {
                    if (cellblock.SubBlocks[i].BlockNumber == subBlockNumber)
                    {
                        addSubblock = false;
                    }
                }
                if (addSubblock)
                {
                    cellblock.SubBlocks.Add(new CellSubBlock()
                    {
                        BlockNumber = subBlockNumber,
                        GroupType = GroupTypeEnum.InteriorCellSubBlock,
                        Cells = new ExtendedList<Cell>()
                    });
                }               

                //Add complete cell
                bool addedCell = false;
                for (int i = 0; i < cellblock.SubBlocks.Count && !addedCell; i++)
                {
                    if (cellblock.SubBlocks[i].BlockNumber == subBlockNumber)
                    {
                        cellblock.SubBlocks[i].Cells.Add(newCell);
                        addedCell = true;
                    }
                }
                if (newCellBlock)
                {
                    myMod.Cells.Add(cellblock);
                }
                //Packin
                Console.WriteLine("Building Record : " + prefix + "_pkn_" + item);
                IFormLink<ITransformGetter> link = new FormKey(env.LoadOrder[0].ModKey, 0x00050FAC).ToLink<ITransformGetter>();
                byte[] barray = new byte[4] { 14, 00, 00, 00 };
                var packin = new PackIn(myMod)
                {
                    MajorFlags = PackIn.MajorFlag.Prefab,
                    EditorID = prefix + "_pkn_" + item,
                    ObjectBounds = new ObjectBounds()
                    {
                        First = new P3Float(-4, -4, -1.767578f),
                        Second = new P3Float(4, 4, 1.767578f)
                    },
                    ODTY = 0,
                    Cell = newCell.ToNullableLink<ICellGetter>(),
                    Version = 0,
                    FNAM = new MemorySlice<byte>(barray),
                    MaterialSwaps = new ExtendedList<IFormLinkGetter<ILayeredMaterialSwapGetter>>()
                };
                myMod.PackIns.Add(packin);
                */
                //Worldspace
                //WaterClear "Water" [WATR:00000018]
                IFormLinkNullable<ILocationGetter> OEJM007Location = new FormKey(env.LoadOrder[0].ModKey, 0x00090A3E).ToNullableLink<ILocationGetter>();
                IFormLinkNullable<IWaterGetter> WaterClear = new FormKey(env.LoadOrder[0].ModKey, 0x00000018).ToNullableLink<IWaterGetter>();
                byte[] FNAM = new byte[1] { 00 };
                byte[] HNAM = new byte[1] { 05 };
                Worldspace worldspace = new Worldspace(myMod)
                {
                    EditorID = prefix + "_world_" + item,
                    Components = new ExtendedList<AComponent>(),
                    Name = prefix + "_world_" + item,
                    Location = OEJM007Location,
                    LodWater = WaterClear,
                    LodWaterHeight = 0,
                    LandDefaults = new WorldspaceLandDefaults() { DefaultLandHeight = -2048, DefaultWaterHeight = -200 },
                    MapData = new WorldspaceMap()
                    {
                        UsableDimensions = new P2Int(0, 0),
                        NorthwestCellCoords = new P2Int16(0, 0),
                        SoutheastCellCoords = new P2Int16(0, 0),
                    },
                    WorldMapCellOffset = new P3Float(0, 0, 0),
                    WorldMapOffsetScale = 1,
                    DistantLodMultiplier = 1,
                    Flags = Worldspace.Flag.SmallWorld,
                    GNAM = 1,
                    FNAM = FNAM,
                    ObjectBoundsMin = new P2Float(-2, -2),
                    ObjectBoundsMax = new P2Float(3, 3),
                    HNAM = HNAM,
                    SubCells = new ExtendedList<WorldspaceBlock>(),
                    TopCell = new Cell(myMod)
                };
                byte[] PNAM = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
                IFormLinkNullable<ISurfaceBlockGetter> OverlayBlockOEJM007World = new FormKey(env.LoadOrder[0].ModKey, 0x00098E69).ToNullableLink<ISurfaceBlockGetter>();
                worldspace.Components.Add(new WorldSpaceOverlayComponent()
                {
                    PNAM = PNAM,
                    SNAM = PNAM,
                    SurfaceBlock = OverlayBlockOEJM007World
                });

                byte[] fourzero = new byte[4] { 0, 0, 0, 0 };
                byte[] NAM4 = new byte[4] { 00, 0xFF, 00, 00 };
                worldspace.Components.Add(new PlanetContentManagerContentPropertiesComponent()
                {
                    ZNAM = 0,
                    YNAM = 0,
                    XNAM = 0,
                    WNAM = 0,
                    VNAM = 1,
                    UNAM = 0,
                    NAM1 = 0,
                    NAM3 = 0,
                    NAM4 = NAM4,
                    NAM5 = 0,
                    NAM6 = 0,
                    NAM7 = 0,
                    NAM8 = 0,
                    NAM9 = 0,
                    Conditions = new ExtendedList<Condition>()
                });

                for(short x = 0; x < 2; x++)
                {
                    for(short y = 0; y < 2; y++)
                    {
                        var wsb = new WorldspaceBlock()
                        {
                            BlockNumberX = x,
                            BlockNumberY = y,
                            GroupType = GroupTypeEnum.ExteriorCellBlock,
                            Items = new ExtendedList<WorldspaceSubBlock>(),                            
                        };
                        wsb.Items.Add(new WorldspaceSubBlock()
                        {
                            BlockNumberX = x,
                            BlockNumberY = y,
                            Items = new ExtendedList<Cell>(),
                            GroupType = GroupTypeEnum.ExteriorCellSubBlock,                            
                        });
                        //Loop here?
                        wsb.Items[0].Items.Add(new Cell(myMod)
                        {
                            Grid = new CellGrid() { Point = new P2Int(0,0)},
                            Flags = Cell.Flag.HasWater,
                            XILS = 1                            
                        });
                        worldspace.SubCells.Add(wsb);
                    }
                }

                myMod.Worldspaces.Add(worldspace);



                //Planet Content Manager Content Node
                IFormLinkNullable<IGlobalGetter> PCM_CellLoadMinDensity_VeryClose = new FormKey(env.LoadOrder[0].ModKey, 0x002A4406).ToNullableLink<IGlobalGetter>();
                var pcmcn = new PlanetContentManagerContentNode(myMod)
                {
                    EditorID = prefix + "_pcmcn_" + item,
                    IOVR = false,
                    Content = worldspace.ToNullableLink<IPlanetContentTargetGetter>()
                };

                myMod.PlanetContentManagerContentNodes.Add(pcmcn);
                //Planet Content Manager Branch Node
                var pcmbn = new PlanetContentManagerBranchNode(myMod)
                {
                    EditorID = prefix + "pcmbn" + item,
                    Components = new ExtendedList<AComponent>(),
                    Conditions = new ExtendedList<Condition>(),
                    Nodes = new ExtendedList<IFormLinkGetter<IPlanetNodeGetter>>(),
                    NAM1 = 2,
                    NAM2 = 0,
                    NAM5 = false
                };
                pcmbn.Nodes.Add(pcmcn);
                byte[] nam4 = new byte[4] { 00, 0xFF, 00, 00 };
                pcmbn.Components.Add(new PlanetContentManagerContentPropertiesComponent()
                {
                    ZNAM = 0,
                    YNAM = 1,
                    XNAM = 0,
                    WNAM = 0,
                    VNAM = 0,
                    UNAM = 0,
                    NAM1 = 0,
                    Global = PCM_CellLoadMinDensity_VeryClose,
                    NAM3 = 0,
                    NAM4 = nam4,
                    NAM5 = 0,
                    NAM6 = 0,
                    NAM7 = 0,
                    NAM8 = 0,
                    NAM9 = 0,
                    Conditions = new ExtendedList<Condition>(),
                });
                myMod.PlanetContentManagerBranchNodes.Add(pcmbn);
            }
            myMod.WriteToBinary(datapath + "\\" + pluginname + ".esm");
            SSFEventLog.EventLogs.Enqueue("Export complete!");
        }
    }
}
