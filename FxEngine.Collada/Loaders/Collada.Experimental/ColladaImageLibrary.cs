using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaImageLibrary : ColladaLibrary
    {
        public List<ColladaImage> Images = new List<ColladaImage>();

        public static ColladaLibrary Parse(XElement item, ColladaParseContext ctx)
        {
            ColladaImageLibrary ret = new ColladaImageLibrary();
            foreach (var iitem in item.Descendants(XName.Get("image", ctx.Ns)))
            {
                ret.Images.Add(ColladaImage.Parse(iitem, ctx));
            }
            return ret;
        }
    }
}


