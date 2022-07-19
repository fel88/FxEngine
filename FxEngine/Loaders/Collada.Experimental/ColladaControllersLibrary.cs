using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaControllersLibrary : ColladaLibrary
    {
        public List<ColladaController> Controllers = new List<ColladaController>();
        public static ColladaControllersLibrary Parse(XElement elem, ColladaParseContext ctx)
        {

            ColladaControllersLibrary ret = new ColladaControllersLibrary();
            foreach (var item in elem.Descendants(XName.Get("controller", ctx.Ns)))
            {
                ret.Controllers.Add(ColladaController.Parse(item, ctx));
            }
            return ret;
        }
    }
}


