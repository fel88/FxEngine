using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaGeometryLibrary : ColladaLibrary
    {
        public List<ColladaGeometry> Geometries = new List<ColladaGeometry>();
        public static ColladaLibrary Parse(XElement item, ColladaParseContext ctx)
        {
            ColladaGeometryLibrary ret = new ColladaGeometryLibrary();
            foreach (var iitem in item.Descendants(XName.Get("geometry", ctx.Ns)))
            {
                ret.Geometries.Add(ColladaGeometry.Parse(iitem, ctx));
            }
            return ret;
        }
    }
}


