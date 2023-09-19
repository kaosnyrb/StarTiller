using Microsoft.VisualBasic.Logging;
using ssf;
using ssf.Generation;
using ssf.IO;
using ssf.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Seedstarfield
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Load block lib
            SSFEventLog.EventLogs = new Queue<string>();
            BlockLib.Instance = new BlockLib
            {
                blocks = Utils.LoadBlockLib("content\\blocks\\")
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
                blocks = generator.Generate(20);
            } while (blocks.Count < 1);

            BlockExporter.Export(blocks);
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
    }
}