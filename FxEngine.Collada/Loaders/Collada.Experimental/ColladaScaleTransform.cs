using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaScaleTransform : ColladaTransform
    {
        public Vector3 Vector;
        public static ColladaTransform Parse(XElement titem, ColladaParseContext ctx)
        {

            ColladaScaleTransform ret = new ColladaScaleTransform();
            var ar = titem.Value.Split(new char[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(z => float.Parse(z, CultureInfo.InvariantCulture)).ToArray();
            ret.Vector = new Vector3(ar[0], ar[1], ar[2]);
            return ret;
        }

        public override Matrix4 GetMatrix()
        {
            return Matrix4.CreateScale(Vector);
        }
    }
}


