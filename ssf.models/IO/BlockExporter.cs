﻿using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.IO
{
    public class BlockExporter
    {
        public static int Export(List<Block> blocks, SeedStarfieldSettings settings)
        {
            SSFEventLog.EventLogs.Enqueue("Exporting to " + settings.ExportPath);
            string pluginname = settings.EspName;
            int count = settings.FormIdOffset;//3428 works

            // Iterate through the files and delete each one
            string[] files = Directory.GetFiles(settings.ExportPath + "/Temporary/");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            // Iterate through the files and delete each one
            files = Directory.GetFiles(settings.ExportPath + "/NavigationMeshes/");
            foreach (string file in files)
            {
                File.Delete(file);
            }

            foreach (var outblock in blocks)
            {
                //SSFEventLog.EventLogs.Enqueue("Exporting block " + outblock.path);
                foreach (var placedobj in outblock.placedObjects)
                {
                    if (!placedobj.EditorID.Contains("ExitBlock"))
                    {
                        placedobj.EditorID = "";//We clear this to stop collisions.
                        placedobj.SkyrimMajorRecordFlags = new List<int>();
                        count++;
                        string formid = count.ToString("X6");
                        placedobj.FormKey = formid + ":" + pluginname;
                        if (placedobj.Placement.Rotation == null) placedobj.Placement.Rotation = "0,0,0";
                        if (placedobj.Placement.Rotation.Contains("E"))
                        {
                            SSFEventLog.EventLogs.Enqueue("Odd Rotation: " + placedobj.Placement.Rotation);
                        }
                        YamlExporter.WriteObjToYamlFile(settings.ExportPath + "/Temporary/" + formid + "_" + pluginname + ".yaml", placedobj);
                    }
                }
                foreach (var navmesh in outblock.navmeshs)
                {
                    count++;
                    string formid = count.ToString("X6");
                    navmesh.VersionControl = 12079;
                    navmesh.FormKey = formid + ":" + pluginname;
                    navmesh.Data.Parent.Parent = "000D62:" + pluginname;
                    YamlExporter.WriteObjToYamlFile(settings.ExportPath + "/NavigationMeshes/" + formid + "_" + pluginname + ".yaml", navmesh);
                }
            }
            SSFEventLog.EventLogs.Enqueue("Export complete!");
            return 1;
        }
    }
}
