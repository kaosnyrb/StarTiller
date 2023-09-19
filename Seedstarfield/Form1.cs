using Microsoft.VisualBasic.Logging;
using ssf;
using ssf.Generation;
using ssf.IO;
using ssf.Models;
using System.Collections.Generic;
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

            SSFEventLog.EventLogs = new Queue<string>();
            BlockLib.Instance = new BlockLib
            {
                blocks = Utils.LoadBlockLib(settings.ContentPath)
            };
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            Mundus generator = new Mundus();
            List<Block> blocks = new List<Block>();
            do
            {
                generator.Setup(BlockLib.Instance);
                blocks = generator.Generate(settings.GenLength);
            } while (blocks.Count < settings.MinBlocks);

            BlockExporter.Export(blocks, settings);
        }
        public void LogEvent(string text)
        {
            textBox1.Text += text;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SSFEventLog.EventLogs.Count > 0)
            {
                textBox1.Text += SSFEventLog.EventLogs.Dequeue() + Environment.NewLine;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process notePad = new Process();

            notePad.StartInfo.FileName = settings.SpriggitCli;
            notePad.StartInfo.Arguments = " deserialize --InputPath \"" + settings.GitModPath + "\" --OutputPath \"" + settings.DataFolder + settings.EspName + "\"";

            notePad.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            YamlExporter.WriteObjToYamlFile("settings.yaml", settings);
        }
    }
}