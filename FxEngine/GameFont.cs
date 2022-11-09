using System.Drawing;
using System.IO;
using System.Linq;

namespace FxEngine
{
    public class GameFont
    {
        public int Id;
        public Bitmap Bitmap;
        public string Path;
        public string Name;

        public void PreLoad(IDataProvider dp)
        {
            var doc = dp.LoadXml(Path);
            //var doc = XDocument.Load(Path);
            var f = doc.Descendants("root").First();
            var path1 = f.Attribute("image").Value;
            var dn = dp.GetDirectoryName(Path);            
            var p1 = System.IO.Path.Combine(dn, path1);
            Bitmap = dp.GetBitmap(p1);
            //Bitmap = Bitmap.FromFile() as Bitmap;
        }
    }
}

