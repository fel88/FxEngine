using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaImage
    {
        public Bitmap Bitmap;
        public string Id;
        public string Name;
        public string Source;

        public static ColladaImage Parse(XElement iitem, ColladaParseContext ctx)
        {
            ColladaImage ret = new ColladaImage();
            var sid = iitem.Attribute("id").Value;
            if (iitem.Attribute("name") != null)
            {
                var name = iitem.Attribute("name").Value;
                ret.Name = name;
            }
            ret.Id = sid;
            
            if (iitem.Attribute("source") != null)
            {
                var source = iitem.Attribute("source").Value;


                var dn = ctx.DataProvider.GetDirectoryName(ctx.Model.FilePath);

                var path = System.IO.Path.Combine(dn, source);
                ret.Source = path;
                ret.Bitmap = Bitmap.FromFile(path) as Bitmap;

                //ret.Bitmap.SetResolution(ctx.Dpi, ctx.Dpi);
            }
            if (iitem.Descendants().Any())
            {
                var frfr = iitem.Descendants(XName.Get("init_from", ctx.Ns)).First();

                var dn = ctx.DataProvider.GetDirectoryName(ctx.Model.FilePath);
                
                var path = System.IO.Path.Combine(dn, frfr.Value);
                ret.Source = path;
                ret.Bitmap = ctx.DataProvider.GetBitmap(path);
                //ret.Bitmap = Bitmap.FromFile(path) as Bitmap;
                //   ret.Bitmap.SetResolution(ctx.Dpi, ctx.Dpi);
            }

            return ret;
        }
    }
}


