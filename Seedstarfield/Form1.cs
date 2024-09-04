using Microsoft.VisualBasic.Logging;
using Mutagen.Bethesda.Starfield;
using ssf;
using ssf.Generation;
using ssf.IO;
using ssf.Models;
using ssf.POI;
using ssf.POI.Cellgen;
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
            //Load settings
            try
            {
                settings = YamlImporter.getObjectFromFile<SeedStarfieldSettings>("settings.yaml");
            }
            catch (Exception ex)
            {
                settings = new SeedStarfieldSettings();
            }
            //Add the generators
            generatordropbox.Items.Add("POI - Fort");
            SSFEventLog.EventLogs = new Queue<string>();
        }

        private async void DoWork()
        {
            StartillerGeneratorInstance gen;
            switch (settings.GeneratorName)
            {
                case "POI - Fort":
                    gen = new FortCellGen();
                    break;

                default:
                    gen = new FortCellGen();
                    break;
            }
            POIBuilder.Setup(gen, settings.seed);
            POIBuilder.Generate(100, settings);
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            textBox1.Text = "";
            settings.seed = int.Parse(Seed.Text);
            await Task.Run(() => DoWork());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (SSFEventLog.EventLogs.Count > 0)
            {
                textBox1.Text += SSFEventLog.EventLogs.Dequeue() + Environment.NewLine;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            YamlExporter.WriteObjToYamlFile("settings.yaml", settings);
        }

        private void randseed_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            Seed.Text = random.Next(int.MaxValue).ToString();
        }

        private async void build10_Click(object sender, EventArgs e)
        {
            int amount = 10;
            for (int i = 0; i < amount; i++)
            {
                textBox1.Text += "Building POI " + (i + 1) + " /" + amount + Environment.NewLine;
                Random random = new Random();
                Seed.Text = random.Next(int.MaxValue).ToString();
                settings.seed = int.Parse(Seed.Text);
                await Task.Run(() => DoWork());
            }
            textBox1.Text += "Batch complete" + Environment.NewLine;
        }

        private void ESMDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var filename = ESMDropdown.SelectedItem.ToString();
            settings.EspName = filename.Substring(0, filename.IndexOf('.'));
        }

        private void generatordropbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.GeneratorName = generatordropbox.SelectedText;
        }

        private void loadesm_Click(object sender, EventArgs e)
        {
            //Get list of ESMs
            ESMDropdown.Items.AddRange(Utils.GetESM().ToArray());

        }
    }
}