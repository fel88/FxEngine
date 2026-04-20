using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaScene
    {
        public string Id;
        public string Name;
        public List<ColladaNode> Nodes = new List<ColladaNode>();

        public static ColladaScene Parse(XElement item, ColladaParseContext ctx)
        {
            ColladaScene ret = new ColladaScene();
            ret.Id = item.Attribute("id").Value;
            ctx.CurrentScene = ret;

            foreach (var nitem in item.Elements(XName.Get("node", ctx.Ns)))
            {
                if (nitem.ToString().Contains("instance_geometry") || nitem.ToString().Contains("instance_controller") || nitem.Elements().Any(z => z.Name.LocalName == "node"))
                {
                    ret.Nodes.Add(ColladaNode.Parse(nitem, ctx));
                }
            }

            return ret;
        }
    }
}


