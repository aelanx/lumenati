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
                ExtractPath = @"C:\s4explore\extract";
                StartupFile = "stage";

                SaveToXML();
            }
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
