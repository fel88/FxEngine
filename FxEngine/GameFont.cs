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
            var fi = new FileInfo(Path);
            var p1 = System.IO.Path.Combine(fi.DirectoryName, path1);
            Bitmap = dp.GetBitmap(p1);
            //Bitmap = Bitmap.FromFile() as Bitmap;
        }
    }
}

