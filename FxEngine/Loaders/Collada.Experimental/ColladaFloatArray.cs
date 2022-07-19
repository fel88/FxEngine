using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaFloatArray : ColladaArray
    {
        public float[] Array;
        public static ColladaFloatArray Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaFloatArray ret = new ColladaFloatArray();
            ret.Count = int.Parse(elem.Attribute("count").Value);
            var arr = elem.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            ret.Id = elem.Attribute("id").Value;

            ret.Array = arr.Select(z => float.Parse(z, CultureInfo.InvariantCulture)).ToArray();

            return ret;
        }
    }
}


