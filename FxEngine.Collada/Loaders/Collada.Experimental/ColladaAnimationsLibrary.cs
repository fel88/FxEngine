using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaAnimationsLibrary : ColladaLibrary
    {
        public List<ColladaAnimation> Animations = new List<ColladaAnimation>();
        public static ColladaAnimationsLibrary Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaAnimationsLibrary ret = new ColladaAnimationsLibrary();
            foreach (var item in elem.Descendants(XName.Get("animation", ctx.Ns)))
            {
                ret.Animations.Add(ColladaAnimation.Parse(item, ctx));
            }
            return ret;
        }
    }
}


