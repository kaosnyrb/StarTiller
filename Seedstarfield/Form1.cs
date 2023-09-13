using Microsoft.VisualBasic.Logging;
using ssf.IO;
using ssf.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace Seedstarfield
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Load block lib
            BlockLib.Instance = new BlockLib();
            BlockLib.Instance.LoadBlockLib("content\\blocks\\");
            textBox1.Text = "Block lib loaded, Block count :" + BlockLib.Instance.blocks.Count.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
//            YamlExporter.WriteObjToYamlFile("placedObject.yaml", obj);
        }
        public void LogEvent(string text)
        {
            textBox1.Text += text;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}