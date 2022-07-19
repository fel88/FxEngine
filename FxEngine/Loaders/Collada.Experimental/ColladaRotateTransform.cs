using OpenTK;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaRotateTransform : ColladaTransform
    {
        public Vector4 Vector;
        public static ColladaTransform Parse(XElement titem, ColladaParseContext ctx)
        {

            ColladaRotateTransform ret = new ColladaRotateTransform();
            var ar = titem.Value.Split(new char[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(z => float.Parse(z, CultureInfo.InvariantCulture)).ToArray();
            ret.Vector = new Vector4(ar[0], ar[1], ar[2], ar[3]);
            return ret;
        }

        public override Matrix4 GetMatrix()
        {
            return Matrix4.CreateFromQuaternion(new Quaternion(Vector.X, Vector.Y, Vector.Z, Vector.W));
        }
    }
}


