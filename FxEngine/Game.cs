using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FxEngine
{
    public class Game
    {
        public Game(string name)
        {
            Name = name;
        }

        public void UpdateSetting(string name, string value)
        {
            var doc = GetConfigDoc();
            var root = doc.Descendants("root").First();
            var ss = doc.Descendants("setting").ToArray();

            if (!ss.Any(z => z.Attribute("name").Value == name))
            {
                var elem = new XElement(XName.Get("setting"));
                elem.SetAttributeValue("name", name);
                elem.SetAttributeValue("value", value);

                root.Add(elem);
            }
            else
            {
                var fr = ss.First(z => z.Attribute("name").Value == name);
                fr.SetAttributeValue("value", value);
            }
            SaveConfig(doc);
        }
        public string GameDirectoryName = "Game";
        public void SaveConfig(XDocument doc)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var path2 = Path.Combine(path, GameDirectoryName, "config.xml");
            doc.Save(path2);
        }
        public XDocument GetConfigDoc()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var p1 = Path.Combine(path, GameDirectoryName);
            if (!Directory.Exists(p1))
            {
                Directory.CreateDirectory(p1);
            }
            var path2 = Path.Combine(path, GameDirectoryName, "config.xml");
            if (!File.Exists(path2))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine("<root>");
                sb.AppendLine("</root>");
                File.WriteAllText(path2, sb.ToString());
            }
            var doc = XDocument.Load(path2);

            return doc;
        }
        public void LoadGameConfig()
        {
            var doc = GetConfigDoc();
            foreach (var item in doc.Descendants("setting"))
            {
                var vl = item.Attribute("value").Value;
                var nm = item.Attribute("name").Value;
                switch (nm)
                {
                    case "levelsOpened":
                        LevelOpened = GameResourcesLibrary.ParseInts(vl).Select(z => (int)z).ToArray();
                        break;
                }
            }
        }

        public string Name { get; set; }
        public GameResourcesLibrary Manager = new GameResourcesLibrary();
        public SceneManager SceneManager = new SceneManager();
        public static int[] LevelOpened;
    }
}
