using System;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaNameArray : ColladaArray
    {
        public string[] Names;

        public static ColladaNameArray Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaNameArray ret = new ColladaNameArray();
            ret.Id = elem.Attribute("id").Value;

            ret.Count = int.Parse(elem.Attribute("count").Value);
            var arr = elem.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            ret.Names = arr.ToArray();
            return ret;
        }

    }
}


