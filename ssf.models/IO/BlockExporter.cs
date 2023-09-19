using ssf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssf.IO
{
    public class BlockExporter
    {
        public static int Export(List<Block> blocks)
        {
            SSFEventLog.EventLogs.Enqueue("Exporting...");
            string pluginname = "bryntest.esp";
            int count = 3000;//3428 works

            // Iterate through the files and delete each one
            string[] files = Directory.GetFiles("Output/Temporary/");
            foreach (string file in files)
            {
                File.Delete(file);
            }
            // Iterate through the files and delete each one
            files = Directory.GetFiles("Output/NavigationMeshes/");
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
                        if (placedobj.Placement.Rotation.Contains("E"))
                        {
                            SSFEventLog.EventLogs.Enqueue("Odd Rotation: " + placedobj.Placement.Rotation);
                        }
                        YamlExporter.WriteObjToYamlFile("Output/Temporary/" + formid + "_" + pluginname + ".yaml", placedobj);
                    }
                }
                foreach (var navmesh in outblock.navmeshs)
                {
                    count++;
                    string formid = count.ToString("X6");
                    navmesh.VersionControl = 12079;
                    navmesh.FormKey = formid + ":" + pluginname;
                    navmesh.Data.Parent.Parent = "000D62:bryntest.esp";
                    YamlExporter.WriteObjToYamlFile("Output/NavigationMeshes/" + formid + "_" + pluginname + ".yaml", navmesh);
                }
            }
            SSFEventLog.EventLogs.Enqueue("Export complete!");
            return 1;
        }
    }
}
