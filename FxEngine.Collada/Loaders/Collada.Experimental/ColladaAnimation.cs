using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaAnimation
    {
        public string Id;

        public List<ColladaSource> Sources = new List<ColladaSource>();

        public ColladaAnimationSampler Sampler;
        public ColladaAnimationChannel Channel;
        public static ColladaAnimation Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaAnimation ret = new ColladaAnimation();

            ret.Id = elem.Attribute("id").Value;
            foreach (var sitem in elem.Descendants(XName.Get("source", ctx.Ns)))
            {
                ret.Sources.Add(ColladaSource.Parse(sitem, ctx));
            }

            ret.Sampler = ColladaAnimationSampler.Parse(elem.Descendants(XName.Get("sampler", ctx.Ns)).First(), ctx);
            ret.Channel = ColladaAnimationChannel.Parse(elem.Descendants(XName.Get("channel", ctx.Ns)).First(), ctx);

            return ret;
        }
    }
}


