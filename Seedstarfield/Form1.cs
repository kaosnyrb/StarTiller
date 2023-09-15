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
            BlockLib.Instance = new BlockLib();
            BlockLib.Instance.LoadBlockLib("content\\blocks\\");

        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*   
               BlockDetails blockDetails = new BlockDetails();
               blockDetails.startpoint = new Vector3(0, 0, 0);
               blockDetails.Connectors = new List<Connector>
               {
                   new Connector()
                   {
                       connectorName = "DweFacadeHallSm1way01",
                       rotation = 0,
                       startpoint = new Vector3(10, 128, 0),
                   }
               };
               blockDetails.BoundingTopLeft = new Vector3(0, 128, 0);
               blockDetails.BoundingBottomRight = new Vector3(64, 0, 0);
               YamlExporter.WriteObjToYamlFile("blockDetails.yaml", blockDetails);*/

            Mundus generator = new Mundus();
            generator.Setup(BlockLib.Instance);
            generator.Generate();

            generator.Export();
        }
        public void LogEvent(string text)
        {
            textBox1.Text += text;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

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