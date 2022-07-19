using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace FxEngine.Fonts
{
    public class Atlas
    {
        public List<CharInfo> Chars = new List<CharInfo>();
        public Font Font;
        public Bitmap AtlasBitmap;

        public void Load(string path1, string path2, bool useEmbedded = true)
        {
            Bitmap bmp = null;
            string xml = null;
            if (useEmbedded)
            {
                var assembly = Assembly.GetEntryAssembly();
                var nms = assembly.GetManifestResourceNames();
                var fn1 = nms.First(z => z.Contains(path1));
                var fn2 = nms.First(z => z.Contains(path2));

                using (Stream stream = assembly.GetManifestResourceStream(fn2))
                using (StreamReader reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
                using (Stream stream = assembly.GetManifestResourceStream(fn1))
                    bmp = (Bitmap)Bitmap.FromStream(stream);
            }
            else
            {
                bmp = (Bitmap)Bitmap.FromFile(path1);
                xml = File.ReadAllText(path2);
            }

            AtlasBitmap = bmp;
            var doc = XDocument.Parse(xml);


            var root = doc.Element("root");
            var fn = root.Attribute("fontName").Value;
            var fsz = float.Parse(root.Attribute("fontSize").Value, CultureInfo.InvariantCulture);

            Font = new Font(fn, fsz);

            foreach (var item in doc.Descendants("tile"))
            {
                var ind = int.Parse(item.Attribute("index").Value);
                if (item.Attribute("char") != null)
                {
                    var ch = item.Attribute("char").Value;
                    ind = (int)ch[0];
                }
                var w = int.Parse(item.Attribute("w").Value);
                var h = int.Parse(item.Attribute("h").Value);
                var x = int.Parse(item.Attribute("x").Value);
                var y = int.Parse(item.Attribute("y").Value);
                var offsetx = int.Parse(item.Attribute("offsetx").Value);
                var offsety = int.Parse(item.Attribute("offsety").Value);
                //var bmp2 = new Bitmap(w, h);
               // using (Graphics gr = Graphics.FromImage(bmp2))
                {
                   // gr.DrawImage(bmp, 0, 0, new System.Drawing.Rectangle(x, y, w, h), GraphicsUnit.Pixel);

                    var ch = new CharInfo();
                    ch.Index = ind;
                    //ch.Bitmap = bmp2;
                    ch.Bound = new Rectangle(x, y, w, h);
                    ch.Offset = new PointF(offsetx, offsety);
                    Chars.Add(ch);
                }
            }
        }
        public class CharInfo
        {
            public int Index;
            public Rectangle Bound;
            //public Bitmap Bitmap;
            public PointF Offset;
        }
    }
}
