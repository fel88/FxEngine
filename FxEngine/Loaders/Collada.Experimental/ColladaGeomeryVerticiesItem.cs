using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaGeomeryVerticiesItem
    {
        public string Id;
        public ColladaInput Input;

        public static ColladaGeomeryVerticiesItem Parse(XElement item, ColladaParseContext ctx)
        {
            ColladaGeomeryVerticiesItem ret = new ColladaGeomeryVerticiesItem();
            ret.Id = item.Attribute("id").Value;
            ret.Input = ColladaInput.Parse(item.Elements(XName.Get("input", ctx.Ns)).First(), ctx);
            return ret;
        }
    }
}


