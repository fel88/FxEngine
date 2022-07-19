using FxEngine.Cameras;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaModel
    {

        public string FilePath;
        public MeshData Data;

        public void InitLibraries()
        {
            foreach (var l in Libraries)
            {
                l.InitGl();
            }
            InitVao();
        }

        public Vao Vao = null;
        public void InitVao()
        {

            ColladaGeometry g;

            foreach (var scene in Scenes)
            {
                foreach (var item in scene.Nodes)
                {
                    if (item.Geometry == null) continue;
                    foreach (var gitem in item.Geometry.PolygonInfos)
                    {
                        gitem.SimpleInit(item.Geometry);
                    }
                }
            }            
        }

        public static Vao CreateVao(MeshData data)
        {
            Vao vao = Vao.create();
            vao.bind(new int[] { });
            vao.createIndexBuffer(data.getIndices());
            vao.createAttribute(0, data.getVertices(), 3);
            vao.createAttribute(1, data.getTextureCoords(), 2);
            vao.createAttribute(2, data.getNormals(), 3);
            //vao.createIntAttribute(3, data.getJointIds(), 3);
            //vao.createAttribute(4, data.getVertexWeights(), 3);
            vao.unbind(new int[] { });
            return vao;
        }

        public void AppendInfoSection()
        {
            var doc = XDocument.Load(FilePath);
            var fr = doc.Descendants().First();
            //fr.Add(new XElement());
        }

        public List<ColladaScene> Scenes = new List<ColladaScene>();
        public List<ColladaLibrary> Libraries = new List<ColladaLibrary>();

        public ColladaGeometryLibrary GeometryLibrary
        {
            get
            {
                return Libraries.OfType<ColladaGeometryLibrary>().First();
            }
        }
        public ColladaControllersLibrary ControllersLibrary
        {
            get
            {
                return Libraries.OfType<ColladaControllersLibrary>().First();
            }
        }
        public ColladaImageLibrary ImagesLibrary
        {
            get
            {
                return Libraries.OfType<ColladaImageLibrary>().First();
            }
        }

        
        public void DrawGeometry(ColladaGeometry node, Matrix4[] mtrs, int shaderProgram, Camera camera, List<Vector3> transformed = null)
        {
            foreach (var item in node.PolygonInfos)
            {
                item.DrawVao(mtrs, shaderProgram, camera);
            }
        }
        public void DrawGeometryNative(ColladaGeometry node, Matrix4[] mtrs, List<Vector3> transformed = null)
        {
            foreach (var item in node.PolygonInfos)
            {
                if (item.Material != null)
                {
                    GL.Color3(Color.White);
                    item.Material.GlApply();
                }


                foreach (var pitem in item.Polygons)
                {
                    if (pitem.Items.Count == 3)
                    {
                        GL.Begin(PrimitiveType.Triangles);
                    }
                    else
                    {
                        GL.Begin(PrimitiveType.Polygon);
                    }

                    int offset = 0;
                    for (int i = 0; i < pitem.Items.Count(); i++)
                    {
                        //foreach (var inpitem in item.PolygonInfo.Inputs)
                        Vector3 vert = new Vector3();
                        for (int j = 0; j < item.Inputs.Count(); j++)
                        {
                            var inpitem = item.Inputs[j];

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
                                if (transformed != null)
                                {
                                    vert = transformed[vitem.Vals[offset + j]];
                                }

                            }
                            if (inpitem.Semantic == "NORMAL")
                            {
                                Vector3 v = new Vector3(arr.AsFloatArray.Array[ind], arr.AsFloatArray.Array[ind + 1], arr.AsFloatArray.Array[ind + 2]);
                                GL.Normal3(v);
                            }
                            if (inpitem.Semantic == "COLOR")
                            {
                                Vector3 v = new Vector3(arr.AsFloatArray.Array[ind], arr.AsFloatArray.Array[ind + 1], arr.AsFloatArray.Array[ind + 2]);
                                GL.Color3(v);
                            }
                            if (inpitem.Semantic == "TEXCOORD")
                            {
                                Vector2 v = new Vector2(arr.AsFloatArray.Array[ind], 1.0f - arr.AsFloatArray.Array[ind + 1]);
                                GL.TexCoord2(v);
                            }
                        }
                        var res = new Vector4(vert, 1);
                        
                        foreach (var mitem in mtrs)
                        {                            
                            res = mitem * res;
                        }
                        
                        GL.Vertex4(res);
                        MinZ = Math.Min(MinZ, vert.Z);
                        MaxZ = Math.Max(MaxZ, vert.Z);
                        //offset += item.PolygonInfo.Inputs.Count;
                    }

                    GL.End();
                }

                //   GL.Disable(EnableCap.Texture2D);
            }

        }

        public float MinZ = float.MaxValue;
        public float MaxZ = float.MinValue;


        public void DrawNodeNative(ColladaNode node)
        {
            var prnts = node.GetAllParents();
            var mtrz = prnts.Select(z => z.OrigMatrix).ToArray();

            node.UpdateMatrix();

            if (node.Geometry != null)
            {
                DrawGeometryNative(node.Geometry, mtrz);
            }

            if (node.Controller != null)
            {
                DrawGeometryNative(node.Controller.Controller.Skin.Source, mtrz, node.Controller.TransformedVectors);
            }

            foreach (var item in node.Nodes)
            {
                DrawNodeNative(item);
            }
        }
        public void DrawNode(ColladaNode node, Camera camera, int shdp)
        {
            var prnts = node.GetAllParents();
            var mtrz = prnts.Select(z => z.OrigMatrix).ToArray();

            node.UpdateMatrix();

            if (node.Geometry != null)
            {
                DrawGeometry(node.Geometry, mtrz, shdp, camera);
            }

            if (node.Controller != null)
            {
                DrawGeometry(node.Controller.Controller.Skin.Source, mtrz, shdp, camera, node.Controller.TransformedVectors);
            }

            foreach (var item in node.Nodes)
            {
                DrawNode(item, camera, shdp);
            }
        }
        public void DrawOldStyle()
        {
            MinZ = float.MaxValue;
            MaxZ = float.MinValue;
            foreach (var scene in Scenes)
            {
                foreach (var node in scene.Nodes)
                {
                    DrawNodeNative(node);
                }
            }
        }
        public void Draw(Camera camera, int shdp)
        {
            MinZ = float.MaxValue;
            MaxZ = float.MinValue;
            foreach (var scene in Scenes)
            {
                foreach (var node in scene.Nodes)
                {
                    DrawNode(node, camera, shdp);
                }
            }
        }
    }
}


