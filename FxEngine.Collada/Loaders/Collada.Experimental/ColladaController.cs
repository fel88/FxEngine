using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaController
    {
        public string Id;
        public string Name;
        public ColladaSkin Skin;

        public static ColladaController Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaController ret = new ColladaController();
            ret.Id = elem.Attribute("id").Value;
            ret.Name = elem.Attribute("name").Value;
            if (elem.Descendants(XName.Get("skin", ctx.Ns)).Any())
            {
                ret.Skin = ColladaSkin.Parse(elem.Descendants(XName.Get("skin", ctx.Ns)).First(), ctx);
            }

            return ret;
        }

    }
}


