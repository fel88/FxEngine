using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FxEngineEditor
{
    public class TileSet
    {
        public List<TileSetSeparator> Items = new List<TileSetSeparator>();
        public void Save(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<tileSet>");
            var verts = Items.Where(z => z.IsVertical).ToArray();
            var horizs = Items.Where(z => !z.IsVertical).ToArray();
            sb.AppendLine("</tileSet>");

            File.WriteAllText(path, sb.ToString());
        }
    }
}
