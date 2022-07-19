using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaVertexWeightsInfo
    {
        public List<ColladaInput> Inputs = new List<ColladaInput>();
        public List<int> Vcount = new List<int>();
        public List<ColladaElement> Polygons = new List<ColladaElement>();
        public int Count;

        public static ColladaVertexWeightsInfo Parse(XElement elem, ColladaParseContext ctx)
        {
            var ns = ctx.Ns;
            ColladaVertexWeightsInfo ret = new ColladaVertexWeightsInfo();

            foreach (var item in elem.Descendants(XName.Get("input", ns)))
            {
                ret.Inputs.Add(ColladaInput.Parse(item, ctx));
            }
            var aa = elem.Element(XName.Get("vcount", ns));
            var ar = aa.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            ret.Vcount.AddRange(ar.Select(z => int.Parse(z)));
            var pp = elem.Elements(XName.Get("v", ns)).First()
                .Value
                .Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(z => int.Parse(z, CultureInfo.InvariantCulture)).ToArray();

            int pos = 0;
            foreach (var item in ret.Vcount)
            {
                ColladaElement pl = new ColladaElement();


                for (int i = 0; i < item; i++)
                {
                    List<int> vl = new List<int>();
                    for (int j = 0; j < ret.Inputs.Count; j++)
                    {
                        var inpitem = ret.Inputs[j];
                        vl.Add(pp[pos + j]);

                    }
                    pos += ret.Inputs.Count;
                    pl.Items.Add(new ColladaElementItem() { Vals = vl.ToArray() });
                }
                ret.Polygons.Add(pl);

            }
            return ret;
        }
    }
}


