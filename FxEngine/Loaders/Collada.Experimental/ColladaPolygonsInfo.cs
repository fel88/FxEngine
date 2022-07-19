using FxEngine.Cameras;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaPolygonsInfo
    {
        int tcount;
        int VAO, VBO;
        public void DrawVao(Matrix4[] mtrz, int shaderProgram, Camera camera)
        {
            GL.PushMatrix();
            for (int i = 0; i < mtrz.Length; i++)
            {
                GL.MultMatrix(ref mtrz[i]);
            }
            GL.UseProgram(shaderProgram);

            var slocation = GL.GetUniformLocation(shaderProgram, "diffuseMap");

            var fr = this;
            if (fr.Material.Effect.Diffuse.Texture != null)
            {
                GL.Uniform1(slocation, 0);

                var tid = fr.Material.Effect.Diffuse.Texture.TextureId;
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, tid);
            }
            Matrix4 matrix;

            GL.GetFloat(GetPName.ModelviewMatrix, out matrix);

            int transformLoc = GL.GetUniformLocation(shaderProgram, "transform");
            int lightDir = GL.GetUniformLocation(shaderProgram, "in_textureCoords");
            Matrix4 m = matrix * camera.ProjectionMatrix;
            GL.UniformMatrix4(transformLoc, false, ref m);

            var clrloc = GL.GetUniformLocation(shaderProgram, "color");
            GL.Uniform4(clrloc, fr.Material.Effect.Diffuse.Color);

            var uc = GL.GetUniformLocation(shaderProgram, "useColors");
            if (fr.Material.Effect.Diffuse.Texture != null)
            {
                GL.Uniform1(uc, 1);
            }
            else
            {
                GL.Uniform1(uc, 0);

            }

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, tcount);

            GL.UseProgram(0);
            GL.PopMatrix();            
        }
        public void DrawVao(ColladaNode node, int shaderProgram, Camera camera)
        {

            var prnts = node.GetAllParents();
            var mtrz = prnts.Select(z => z.OrigMatrix).ToArray();
            DrawVao(mtrz, shaderProgram, camera);

        }
        public void SimpleInit(ColladaGeometry geom)
        {
            var md = GetMeshData(geom);
            
            List<float> vres = new List<float>();
            var text = md.getTextureCoords();
            var nrmls = md.getNormals();
            var vertices = md.getVertices();
            int cntt = 0;
            for (int i = 0; i < md.getVertices().Count(); i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    vres.Add(vertices[i + j]);
                }
                for (int j = 0; j < 2; j++)
                {
                    if (text.Any())
                    {
                        vres.Add(text[cntt++]);
                    }
                    else
                    {
                        vres.Add(0);
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    vres.Add(nrmls[i + j]);
                }

            }
            tcount = md.getVertices().Count() / 3;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            
            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vres.Count(), vres.ToArray(), BufferUsageHint.StaticDraw);

         
            GL.VertexAttribPointer(
               0,                
               3,                
               VertexAttribPointerType.Float,    
               false,           
              8 * sizeof(float),     
               0           
            );
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(
               1,                
               2,                
               VertexAttribPointerType.Float,   
               false,         
              8 * sizeof(float),            
               3 * sizeof(float)           
            );
            GL.EnableVertexAttribArray(1);


            GL.VertexAttribPointer(
               2,               
               3,               
               VertexAttribPointerType.Float,           
               false,   
              8 * sizeof(float),
               5 * sizeof(float)
            );
            GL.EnableVertexAttribArray(2);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);
        }
        public ColladaMaterial Material;
        public int Count;
        public List<ColladaElement> Polygons = new List<ColladaElement>();
        public List<ColladaInput> Inputs = new List<ColladaInput>();
        public List<int> Vcount = new List<int>();
        public Vao Vao = null;
        public void InitVao(ColladaNode node)
        {
            var md = GetMeshData(node.Geometry);
            Vao = ColladaModel.CreateVao(md);
        }

        public MeshData GetMeshData(ColladaGeometry node)
        {
            List<float> ff = new List<float>();
            List<float> txf = new List<float>();
            List<float> nrmls = new List<float>();
            List<int> indics = new List<int>();
            List<int> jntidc = new List<int>();
            List<float> vertWeights = new List<float>();

            foreach (var pitem in Polygons)
            {

                int offset = 0;

                for (int i = 0; i < pitem.Items.Count(); i++)
                {
                    
                    Vector3 vert = new Vector3();
                    for (int j = 0; j < Inputs.Count(); j++)
                    {
                        var inpitem = Inputs[j];

                        var nm = inpitem.Source.Replace("#", "");
                        ColladaSource arr = null;
                        if (node.VerticiesItem != null)
                        {
                            if (node.VerticiesItem.Id == nm)
                            {
                                nm = node.VerticiesItem.Input.Source.Replace("#", "");
                            }
                        }
                        arr = node.Sources.First(z => z.Id == nm);

                        var vitem = pitem.Items[i];


                        var ind = vitem.Vals[offset + j] * arr.Stride;


                        if (inpitem.Semantic == "VERTEX")
                        {
                            vert = new Vector3(arr.AsFloatArray.Array[ind], arr.AsFloatArray.Array[ind + 1], arr.AsFloatArray.Array[ind + 2]);
                        }
                        if (inpitem.Semantic == "NORMAL")
                        {
                            Vector3 v = new Vector3(arr.AsFloatArray.Array[ind], arr.AsFloatArray.Array[ind + 1], arr.AsFloatArray.Array[ind + 2]);
                            nrmls.Add(v.X);
                            nrmls.Add(v.Y);
                            nrmls.Add(v.Z);


                        }
                        if (inpitem.Semantic == "COLOR")
                        {
                            Vector3 v = new Vector3(arr.AsFloatArray.Array[ind], arr.AsFloatArray.Array[ind + 1], arr.AsFloatArray.Array[ind + 2]);

                        }
                        if (inpitem.Semantic == "TEXCOORD")
                        {
                            Vector2 v = new Vector2(arr.AsFloatArray.Array[ind], 1.0f - arr.AsFloatArray.Array[ind + 1]);

                            txf.Add(v.X);
                            txf.Add(v.Y);

                        }

                    }


                    var res = new Vector4(vert, 1);

                    ff.Add(res.X);
                    ff.Add(res.Y);
                    ff.Add(res.Z);

                    indics.Add(ff.Count() / 3);
                }
            }

            MeshData md = new MeshData(ff.ToArray(), txf.ToArray(), nrmls.ToArray(), indics.ToArray(), jntidc.ToArray(), vertWeights.ToArray());
            return md;
        }

        public static ColladaPolygonsInfo Parse(XElement elem, ColladaParseContext ctx)
        {
            var ns = ctx.Ns;
            ColladaPolygonsInfo ret = new ColladaPolygonsInfo();
            bool isPolylist = elem.Name.LocalName == "polylist";
            if (elem.Attribute("material") != null)
            {
                var val = elem.Attribute("material").Value.Replace("#", "");
                var fr = ctx.Model.Libraries.OfType<ColladaMaterialLibrary>().First();
                var mfr = fr.Materials.First(z => z.Id == val || z.Name == val);
                ret.Material = mfr;
            }
            foreach (var item in elem.Descendants(XName.Get("input", ns)))
            {
                ret.Inputs.Add(ColladaInput.Parse(item, ctx));
            }
            if (elem.Name.LocalName == "triangles")
            {
                var cnt = int.Parse(elem.Attribute("count").Value);

                var pp = elem.Elements(XName.Get("p", ns)).First()
                    .Value
                    .Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(z => int.Parse(z, CultureInfo.InvariantCulture)).ToArray();

                int pos = 0;
                for (int z = 0; z < cnt; z++)
                {

                    ColladaElement pl = new ColladaElement();
                    List<int> vl = new List<int>();

                    for (int i = 0; i < 3; i++)
                    {
                        vl.Clear();
                        for (int j = 0; j < ret.Inputs.Count; j++)
                        {
                            var inpitem = ret.Inputs[j];

                            vl.Add(pp[pos + j]);

                        }
                        pl.Items.Add(new ColladaElementItem() { Vals = vl.ToArray() });

                        pos += ret.Inputs.Count;
                    }
                    ret.Polygons.Add(pl);
                }
            }
            else
            if (isPolylist)
            {

                var aa = elem.Element(XName.Get("vcount", ns));
                var ar = aa.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                ret.Vcount.AddRange(ar.Select(z => int.Parse(z)));
                var pp = elem.Elements(XName.Get("p", ns)).First()
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

            }
            else
            {
                foreach (var item in elem.Elements(XName.Get("p", ns)))
                {
                    var spl = item.Value.Split(new char[] { ' ', '\n' }).ToArray();
                    //todo : parse inputs..
                    //[vertex] [normal]
                    var pl = new ColladaElement();
                    ret.Polygons.Add(pl);
                    for (int i = 0; i < spl.Count(); i += ret.Inputs.Count)
                    {

                        List<int> ll = new List<int>();

                        for (int j = 0; j < ret.Inputs.Count; j++)
                        {
                            var v1 = int.Parse(spl[i + j], CultureInfo.InvariantCulture);
                            ll.Add(v1);

                        }
                        pl.Items.Add(new ColladaElementItem() { Vals = ll.ToArray() });
                    }
                }
            }
            return ret;
        }
    }
}
