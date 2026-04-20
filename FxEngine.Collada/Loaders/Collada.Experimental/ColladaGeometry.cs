using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaGeometry
    {
        public string Id;
        public string Name;
        public List<ColladaSource> Sources = new List<ColladaSource>();
        public List<ColladaPolygonsInfo> PolygonInfos = new List<ColladaPolygonsInfo>();

        public ColladaGeomeryVerticiesItem VerticiesItem;


        public static ColladaGeometry Parse(XElement elem, ColladaParseContext ctx)
        {
            var ns = ctx.Ns;
            ColladaGeometry g = new ColladaGeometry();
            g.Id = elem.Attribute("id").Value;
            g.Name = elem.Attribute("name").Value;
            foreach (var item in elem.Descendants(XName.Get("source", ns)))
            {
                g.Sources.Add(ColladaSource.Parse(item, ctx));
            }

            foreach (var item in elem.Descendants(XName.Get("polygons", ns)))
            {
                g.PolygonInfos.Add(ColladaPolygonsInfo.Parse(item, ctx));
            }
            foreach (var item in elem.Descendants(XName.Get("polylist", ns)))
            {
                g.PolygonInfos.Add(ColladaPolygonsInfo.Parse(item, ctx));
            }
            foreach (var item in elem.Descendants(XName.Get("triangles", ns)))
            {
                g.PolygonInfos.Add(ColladaPolygonsInfo.Parse(item, ctx));
            }



            foreach (var item in elem.Descendants(XName.Get("vertices", ctx.Ns)))
            {
                g.VerticiesItem = ColladaGeomeryVerticiesItem.Parse(item, ctx);
            }

            return g;
        }
    }
}


