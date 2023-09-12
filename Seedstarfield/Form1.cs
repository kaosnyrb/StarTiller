using ssf.IO;
using ssf.Models;
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
            ssf.Models.PlacedObject placedObject = new ssf.Models.PlacedObject()
            {
                MutagenObjectType = "PlacedObject",
                FormKey = "1FEFBB:Undaunted.esp",
                EditorID = "DweWallSconceDUPLICATE013",
                FormVersion = 44,
                Base = "05ADDD:Skyrim.esm",
                Scale = 1,
                Placement = new ssf.Models.Placement()
                {
                    Position = "-256, -608, 128",
                    Rotation = "0, 0, 4.712389"
                }
            };

            var result = File.ReadAllText("C:\\git\\Undaunted\\spriggit\\Cells\\5\\3\\01_Undaunted_Smallcave\\Temporary\\BoneHumanBloodyLegDUPLICATE002.yaml");
            PlacedObject obj = YamlImporter.getObjectFromYaml<PlacedObject>(result);

            YamlExporter.WriteObjToYamlFile("placedObject.yaml", placedObject);

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