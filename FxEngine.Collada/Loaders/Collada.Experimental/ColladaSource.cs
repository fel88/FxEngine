using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaSource
    {
        public string Id;
        public ColladaArray Array;
        public ColladaFloatArray AsFloatArray
        {
            get
            {
                return Array as ColladaFloatArray;
            }
        }
        public string FloatArrayId;

        public int Stride;
        public int Count;

        public static ColladaSource Parse(XElement felem, ColladaParseContext ctx)
        {

            ColladaSource ret = new ColladaSource();
            ret.Id = felem.Attribute("id").Value;
            foreach (var item in felem.Elements())
            {
                if (item.Name.LocalName.ToLower() == "float_array")
                {
                    ret.Array = ColladaFloatArray.Parse(item, ctx);
                }
                if (item.Name.LocalName.ToLower() == "name_array")
                {
                    ret.Array = ColladaNameArray.Parse(item, ctx);
                }
            }

            var d1 = felem.Descendants(XName.Get("accessor", ctx.Ns)).First();
            List<string> types = new List<string>();
            foreach (var item in d1.Descendants(XName.Get("param", ctx.Ns)))
            {
                types.Add(item.Attribute("type").Value);
            }
            if (types.Count == 1 && types[0] == "float4x4")
            {
                ret.Accessor = new ColladaMatrix4Accessor(ret);
            }
            else if (types.Count() == 1 && types[0] == "name")
            {
                ret.Accessor = new ColladaNameAccessor(ret);
            }
            else
            {
                ret.Accessor = new ColladaFloatAccessor(ret);
            }
            var s = int.Parse(d1.Attribute("stride").Value);
            ret.Stride = s;
            ret.Count = int.Parse(d1.Attribute("count").Value);

            return ret;
        }

        public IColladaAccessor Accessor;


    }
}


