using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace FxEngineEditor
{
    public class Config
    {
        public List<string> RecentLibraries = new List<string>();

        public bool IsDirty { get; set; } = false;

        public void Load()
        {
            var doc = XDocument.Load("config.xml");
            foreach (var item in doc.Descendants("setting"))
            {
                var nm = item.Attribute("name").Value;
                var vl = item.Attribute("value").Value;
            }
            foreach (var item in doc.Descendants("recent"))
            {
                var nm = item.Attribute("path").Value;
                RecentLibraries.Add(nm);
            }
            IsDirty = false;
        }

        public void Save()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<?xml version=\"1.0\"?>");
            sb.AppendLine("<root>");
            sb.AppendLine("<recents>");
            foreach (var item in RecentLibraries)
            {
                sb.AppendLine($"<recent path=\"{item}\"/>");
            }
            sb.AppendLine("</recents>");

            sb.AppendLine("</root>");
            File.WriteAllText("config.xml", sb.ToString());
        }
    }
}
