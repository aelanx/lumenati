using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Lumenati
{
    class Preferences
    {
        public string ExtractPath;
        public string StartupFile;

        public static Preferences Instance;

        public static void Init()
        {
            Instance = new Preferences();
        }

        public string XMLPath
        {
            get
            {
                return Path.Combine(Application.StartupPath, "preferences.xml");
            }
        }

        Preferences()
        {
            if (File.Exists(XMLPath))
            {
                LoadFromXML();
            }
            else
            {
                CreateXML();
            }
        }

        void CreateXML()
        {
            var res = MessageBox.Show("In order to load some vital files (fonts and whatnot), I'll need to use your Sm4shExplorer extract directory. Point me there?", "Select Extract Folder", MessageBoxButtons.YesNo);


            if (res == DialogResult.Yes)
            {
                var fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ExtractPath = fbd.SelectedPath;
                    SaveToXML();
                    return;
                }
            }

            MessageBox.Show("Okay, be that way. Just know that this probably isn't going to work correctly and I'm just gonna ask again at next startup.", "Wow...", MessageBoxButtons.OK);
        }

        void LoadFromXML()
        {
            var doc = new XmlDocument();
            doc.Load(XMLPath);
            var root = doc.SelectSingleNode("Preferences");

            ExtractPath = root.SelectSingleNode("ExtractPath").InnerText;

            var startupFileNode = root.SelectSingleNode("StartupFile");
            if (startupFileNode != null)
                StartupFile = startupFileNode.InnerText;
        }

        void SaveToXML()
        {
            var doc = new XDocument(
                new XElement("Preferences",
                    new XElement("ExtractPath", ExtractPath),
                    new XElement("StartupFile", StartupFile)
                )
            );

            doc.Save(XMLPath);
        }
    }
}
