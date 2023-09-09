using Mutagen.Bethesda.Starfield;
using ssf.IO;
using System.Numerics;

namespace Seedstarfield
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormList rec = new FormList();


            YamlExporter.WriteObjToYamlFile("rec.yaml", rec);

        }
    }
}