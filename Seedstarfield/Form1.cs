using Microsoft.VisualBasic.Logging;
using ssf;
using ssf.Generation;
using ssf.IO;
using ssf.Models;
using ssf.POI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using YamlDotNet.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Seedstarfield
{
    public partial class Form1 : Form
    {
        public SeedStarfieldSettings settings;

        public Form1()
        {
            InitializeComponent();

            settings = YamlImporter.getObjectFromFile<SeedStarfieldSettings>("settings.yaml");

            //refresh ui
            GenLength_TextBox.Text = settings.GenLength.ToString();
            MinBlocks_Text.Text = settings.MinBlocks.ToString();
            FormIdOffset_Text.Text = settings.FormIdOffset.ToString();
            espname_text.Text = settings.EspName;

            SSFEventLog.EventLogs = new Queue<string>();
            BlockLib.Instance = new BlockLib
            {
                blocks = Utils.LoadBlockLib(settings.ContentPath)
            };
        }

        private async void DoWork()
        {
            
            Mundus generator = new Mundus();
            List<Block> blocks = new List<Block>();
            POIBuilder.Setup(BlockLib.Instance, settings.seed);
            POIBuilder.Generate(100, settings);

            /*
            do
            {
                generator.Setup(BlockLib.Instance, settings.seed);
                blocks = generator.Generate(settings.GenLength);
            } while (blocks.Count < settings.MinBlocks);
            ESMExporter.Export(blocks, settings);*/
            //            BlockExporter.Export(blocks, settings);
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            textBox1.Text = "";
            settings.cellname = CellName.Text;
            settings.seed = int.Parse(Seed.Text);
            await Task.Run(() => DoWork());
        }
        public void LogEvent(string text)
        {
            textBox1.Text += text;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            while (SSFEventLog.EventLogs.Count > 0)
            {
                textBox1.Text += SSFEventLog.EventLogs.Dequeue() + Environment.NewLine;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process notePad = new Process();

            notePad.StartInfo.FileName = settings.SpriggitCli;
            notePad.StartInfo.Arguments = " deserialize --InputPath \"" + settings.GitModPath + "\" --OutputPath \"" + settings.DataFolder + settings.EspName + "\"";

            SSFEventLog.EventLogs.Enqueue("Running SpriggitCli with : " + notePad.StartInfo.Arguments);
            notePad.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            YamlExporter.WriteObjToYamlFile("settings.yaml", settings);
        }

        private void GenLength_TextBox_TextChanged(object sender, EventArgs e)
        {
            settings.GenLength = int.Parse(GenLength_TextBox.Text);
        }

        private void MinBlocks_Text_TextChanged(object sender, EventArgs e)
        {
            settings.MinBlocks = int.Parse(MinBlocks_Text.Text);
        }

        private void FormIdOffset_Text_TextChanged(object sender, EventArgs e)
        {
            settings.FormIdOffset = int.Parse(FormIdOffset_Text.Text);
        }

        private void espname_text_TextChanged(object sender, EventArgs e)
        {
            settings.EspName = espname_text.Text;
        }
    }
}