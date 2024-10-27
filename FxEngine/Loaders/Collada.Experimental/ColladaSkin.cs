using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaSkin
    {


        public Matrix4 BindShapeMatrix;
        public List<ColladaSource> Sources = new List<ColladaSource>();
        public ColladaJointsInfo Joints;
        public ColladaVertexWeightsInfo Weights;

        public ColladaGeometry Source;

        public static ColladaSkin Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaSkin ret = new ColladaSkin();
            var arr = elem.Descendants(XName.Get("bind_shape_matrix", ctx.Ns)).First().Value.Split(new char[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(z => float.Parse(z, CultureInfo.InvariantCulture)).ToArray();
            ret.BindShapeMatrix = new Matrix4(
                    new Vector4(arr[0], arr[1], arr[2], arr[3]),
                    new Vector4(arr[4], arr[5], arr[6], arr[7]),
                    new Vector4(arr[8], arr[9], arr[10], arr[11]),
                    new Vector4(arr[12], arr[13], arr[14], arr[15])
                    );

            var a = elem.Attribute("source").Value.Replace("#", "");
            ret.Source = ctx.Model.GeometryLibrary.Geometries.First(z => z.Id == a);
            foreach (var item in elem.Descendants(XName.Get("source", ctx.Ns)))
            {
                ret.Sources.Add(ColladaSource.Parse(item, ctx));
            }

            ret.Weights = ColladaVertexWeightsInfo.Parse(elem.Descendants(XName.Get("vertex_weights", ctx.Ns)).First(), ctx);
            ret.Joints = ColladaJointsInfo.Parse(elem.Descendants(XName.Get("joints", ctx.Ns)).First(), ctx);



            return ret;
        }
    }
}


