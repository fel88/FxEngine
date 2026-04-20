using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaMaterialLibrary : ColladaLibrary
    {
        public List<ColladaMaterial> Materials = new List<ColladaMaterial>();
        public static ColladaLibrary Parse(XElement item, ColladaParseContext ctx)
        {
            ColladaMaterialLibrary ret = new ColladaMaterialLibrary();
            foreach (var iitem in item.Descendants(XName.Get("material", ctx.Ns)))
            {
                ret.Materials.Add(ColladaMaterial.Parse(iitem, ctx));
            }
            return ret;
        }
    }
}


