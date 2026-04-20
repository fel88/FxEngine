using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaInput
    {
        public string Semantic;
        public string Source;
        public int Offset;
        public static ColladaInput Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaInput ret = new ColladaInput();
            ret.Semantic = elem.Attribute("semantic").Value;
            ret.Source = elem.Attribute("source").Value;
            if (elem.Attribute("offset") != null)
            {
                ret.Offset = int.Parse(elem.Attribute("offset").Value);
            }
            else if (elem.Attribute("idx") != null)
            {
                ret.Offset = int.Parse(elem.Attribute("idx").Value);
            }
            return ret;
        }
    }
}


