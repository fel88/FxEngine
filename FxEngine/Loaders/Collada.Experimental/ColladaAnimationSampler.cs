using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaAnimationSampler
    {
        public string Id;
        public List<ColladaInput> Inputs = new List<ColladaInput>();

        public static ColladaAnimationSampler Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaAnimationSampler ret = new ColladaAnimationSampler();
            ret.Id = elem.Attribute("id").Value;
            foreach (var item in elem.Descendants(XName.Get("input", ctx.Ns)))
            {
                ret.Inputs.Add(ColladaInput.Parse(item, ctx));
            }

            return ret;
        }
    }
}


