using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaAnimationChannel
    {
        public string Source;
        public string Target;

        public static ColladaAnimationChannel Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaAnimationChannel ret = new ColladaAnimationChannel();
            ret.Source = elem.Attribute("source").Value;
            ret.Target = elem.Attribute("target").Value;
            return ret;
        }
    }
}


