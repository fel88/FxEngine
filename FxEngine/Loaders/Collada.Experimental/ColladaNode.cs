using OpenTK;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaNode
    {
        public string Id;
        public string Name;
        public string Type;
        public ColladaNode[] GetAllParents(bool withRoot = false)
        {
            ColladaNode ptr = this;
            List<ColladaNode> prnts = new List<ColladaNode>();
            while (ptr != null)
            {
                prnts.Add(ptr);
                ptr = ptr.Parent;
            }
            return prnts.ToArray();

        }
        public ColladaNode[] GetAllChilds(bool withRoot = false)
        {
            List<ColladaNode> ret = new List<ColladaNode>();
            if (withRoot)
            {
                ret.Add(this);
            }
            ret.AddRange(Nodes);
            foreach (var item in Nodes)
            {
                ret.AddRange(item.GetAllChilds());
            }
            return ret.ToArray();
        }

        public ColladaNode Parent;

        public ColladaGeometry Geometry;
        public ColladaInstanceController Controller;
        public List<ColladaTransform> Transforms = new List<ColladaTransform>();
        public Matrix4 OrigMatrix;
        public Matrix4 Matrix;
        public ColladaJoint Joint;



        public List<ColladaNode> Nodes = new List<ColladaNode>();
        public static ColladaNode Parse(XElement item, ColladaParseContext ctx)
        {
            ColladaNode ret = new ColladaNode();
            ret.Id = item.Attribute("id").Value;
            if (item.Attribute("type") != null)
            {
                ret.Type = item.Attribute("type").Value;
            }
            if (ret.Type == "JOINT")
            {
                ret.Joint = new ColladaJoint(ret);


            }
            if (item.Attribute("name") != null)
            {
                ret.Name = item.Attribute("name").Value;
            }

            foreach (var titem in item.Elements())
            {
                if (titem.Name.LocalName == "translate")
                {
                    ret.Transforms.Add(ColladaTranslateTransform.Parse(titem, ctx));
                }
                if (titem.Name.LocalName == "rotate")
                {
                    ret.Transforms.Add(ColladaRotateTransform.Parse(titem, ctx));
                }
                if (titem.Name.LocalName == "scale")
                {
                    ret.Transforms.Add(ColladaScaleTransform.Parse(titem, ctx));
                }
            }

            if (ret.Transforms.Any())
            {
                ret.OrigMatrix = Matrix4.Identity;
                foreach (var titem in ret.Transforms)
                {
                    ret.OrigMatrix *= titem.GetMatrix();
                }
            }
            if (item.Elements(XName.Get("instance_geometry", ctx.Ns)).Any())
            {
                var fr = item.Elements(XName.Get("instance_geometry", ctx.Ns)).First();
                var url = fr.Attribute("url").Value.Replace("#", "");
                ret.Geometry = ctx.Model.GeometryLibrary.Geometries.First(z => z.Id == url);
            }


            if (item.Elements(XName.Get("matrix", ctx.Ns)).Any())
            {
                var fr = item.Elements(XName.Get("matrix", ctx.Ns)).First();
                var arr = fr.Value.Split(new char[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(z => float.Parse(z, CultureInfo.InvariantCulture)).ToArray();

                ret.OrigMatrix = new Matrix4(
                    new Vector4(arr[0], arr[1], arr[2], arr[3]),
                    new Vector4(arr[4], arr[5], arr[6], arr[7]),
                    new Vector4(arr[8], arr[9], arr[10], arr[11]),
                    new Vector4(arr[12], arr[13], arr[14], arr[15])
                    );

                ret.Matrix = new Matrix4(ret.OrigMatrix.Row0, ret.OrigMatrix.Row1, ret.OrigMatrix.Row2, ret.OrigMatrix.Row3);
            }
            if (ret.Joint != null)
            {
                Matrix4 mm = ret.OrigMatrix;
                mm.Transpose();
                ret.Joint.LocalBindTransform = mm;
            }
            foreach (var zitem in item.Elements(XName.Get("node", ctx.Ns)))
            {
                ret.Nodes.Add(ColladaNode.Parse(zitem, ctx));
                ret.Nodes.Last().Parent = ret;
                if (ret.Type == "JOINT")
                {
                    ret.Nodes.Last().Joint.Parent = ret.Joint;
                    ret.Joint.Childs.Add(ret.Nodes.Last().Joint);
                }
            }
            if (item.Elements(XName.Get("instance_controller", ctx.Ns)).Any())
            {
                var fr = item.Elements(XName.Get("instance_controller", ctx.Ns)).First();
                var url = fr.Attribute("url").Value.Replace("#", "");
                ret.Controller = ColladaInstanceController.Parse(fr, ctx);
            }
            return ret;
        }

        public void UpdateMatrix()
        {
            var prnts = GetAllParents();
            var mtrz = prnts.Select(z => z.OrigMatrix).ToArray();

        }
    }
}


