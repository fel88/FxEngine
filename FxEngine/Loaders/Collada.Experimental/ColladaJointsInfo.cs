using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaJointsInfo
    {
        public List<ColladaInput> Inputs = new List<ColladaInput>();

        public static ColladaJointsInfo Parse(XElement elem, ColladaParseContext ctx)
        {
            var ns = ctx.Ns;
            ColladaJointsInfo ret = new ColladaJointsInfo();

            foreach (var item in elem.Descendants(XName.Get("input", ns)))
            {
                ret.Inputs.Add(ColladaInput.Parse(item, ctx));
            }
            return ret;
        }
    }
}


