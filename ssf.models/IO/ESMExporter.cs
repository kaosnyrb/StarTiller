using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda;
using ssf.Manipulators;
using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins;
using Noggog;
using DynamicData;

namespace ssf.IO
{
    public class ESMExporter
    {
        public static int Export(List<Block> blocks, SeedStarfieldSettings settings)
        {
            string pluginname = settings.EspName;
            string datapath = "";

            string prefix = "Test";
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
                //Create the Cell
                IFormLinkNullable<IImageSpaceGetter> DefaultImagespacePackin = new FormKey(env.LoadOrder[0].ModKey, 0x0006AD68).ToNullableLink<IImageSpaceGetter>();
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
                    WaterHeight = 0,
                    XILS = 1.0f,
                    XCLAs = new ExtendedList<CellXCLAItem>()
                    {
                        new CellXCLAItem()
                        {
                            XCLA = 1,
                            XCLD = "Default Layer Name 1"
                        },
                        new CellXCLAItem()
                        {
                            XCLA = 2,
                            XCLD = "Default Layer Name 2"
                        },
                        new CellXCLAItem()
                        {
                            XCLA = 3,
                            XCLD = "Default Layer Name 3"
                        },
                        new CellXCLAItem()
                        {
                            XCLA = 4,
                            XCLD = "Default Layer Name 4"
                        },
                    },
                    ImageSpace = DefaultImagespacePackin,

                };
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

                //Export
                foreach (var outblock in blocks)
                {
                    SSFEventLog.EventLogs.Enqueue("Exporting block " + outblock.path);
                    //IFormLink<IPlaceableObjectGetter> OutpostGroupPackinDummy = new FormKey(env.LoadOrder[0].ModKey, 0x00015804).ToLink<IPlaceableObjectGetter>();
                    //IFormLink<IPlaceableObjectGetter> PrefabPackinPivotDummy = new FormKey(env.LoadOrder[0].ModKey, 0x0003F808).ToLink<IPlaceableObjectGetter>();
                    //IFormLink<IKeywordGetter> UpdatesDynamicNavmeshKeyword = new FormKey(env.LoadOrder[0].ModKey, 0x00140158).ToLink<IKeywordGetter>();

                    /*
                    IFormLink<IPlaceableObjectGetter> COCMarkerHeading = new FormKey(env.LoadOrder[0].ModKey, 0x00000032).ToLink<IPlaceableObjectGetter>();

                    foreach (var placedobj in outblock.placedObjects)
                    {
                        if (!placedobj.EditorID.Contains("ExitBlock"))
                        {
                            placedobj.EditorID = "";//We clear this to stop collisions.
                            if (placedobj.Placement.Rotation == null) placedobj.Placement.Rotation = "0,0,0";
                            if (placedobj.Placement.Position == null) placedobj.Placement.Position = "0,0,0";
                            if (placedobj.Placement.Rotation.Contains("E"))
                            {
                                SSFEventLog.EventLogs.Enqueue("Odd Rotation: " + placedobj.Placement.Rotation);
                            }
                            var rot = Utils.ConvertStringToVector3(placedobj.Placement.Rotation);
                            if (rot.Y > 10 || rot.Y < -10)
                            {
                                SSFEventLog.EventLogs.Enqueue("Odd Rotation: " + placedobj.Placement.Rotation);
                            }
                            var pos = Utils.ConvertStringToVector3(placedobj.Placement.Position);
                            var basemod = placedobj.Base.Split(':');
                            int i = Convert.ToInt32("0x" + basemod[0], 16);
                            var formkey = new FormKey(basemod[1], (uint)i);
                            IFormLink<IPlaceableObjectGetter> baseobj = formkey.ToLink<IPlaceableObjectGetter>();

                            SSFEventLog.EventLogs.Enqueue("pos" + " " + pos.X + " " + pos.Y + " " + pos.Z);
                            SSFEventLog.EventLogs.Enqueue("rot" + " " + rot.X + " " + rot.Y + " " + rot.Z);
                            newCell.Temporary.Add(new Mutagen.Bethesda.Starfield.PlacedObject(myMod)
                            {
                                Base = baseobj,
                                Position = new P3Float(pos.X, pos.Y, pos.Z),
                                Rotation = new P3Float(rot.X, rot.Y, rot.Z)
                            });
                        }
                    }
                    /*
                    var mesh = NavMeshUtils.BuildNavmesh(blocks);
                    NavmeshGeometry navmeshGeometry = new NavmeshGeometry();
                    newCell.NavigationMeshes.Add(new NavigationMesh(myMod)
                    {
                        NavmeshGeometry = navmeshGeometry                        
                    });*/
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
            }
            

            myMod.WriteToBinary(datapath + "\\" + pluginname + ".esm");
            SSFEventLog.EventLogs.Enqueue("Export complete!");
            return 1;
        }
    }
}
