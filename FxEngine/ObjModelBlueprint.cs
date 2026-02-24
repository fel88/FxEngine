using FxEngine.Loaders.OBJ;
using FxEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace FxEngine
{
    public class ObjModelBlueprint : ModelBlueprint
    {
        public List<ObjVolume> Objs;
        public ObjModelBlueprint(string name, IEnumerable<ObjVolume> vols)
        {
            Name = name;
            Objs = new List<ObjVolume>();
            Objs.AddRange(vols);
            VaoModel = new VaoModel();
            VaoModel.ModelInit(Objs.ToArray());
            Vector3d res = new Vector3();
            int cnt = 0;
            foreach (var item in vols)
            {
                foreach (var fitem in item.faces)
                {
                    var tx = fitem.Vertexes.Sum(z => z.Position.X);
                    var ty = fitem.Vertexes.Sum(z => z.Position.Y);
                    var tz = fitem.Vertexes.Sum(z => z.Position.Z);
                    res.X += tx;
                    res.Y += ty;
                    res.Z += tz;
                    cnt++;
                }
            }
            res /= cnt;

            foreach (var item in vols)
            {
                foreach (var fitem in item.faces)
                {
                    for (int i = 0; i < fitem.Vertexes.Count(); i++)
                    {
                        //                        fitem.Vertexes[i].Position -= res;
                    }
                }
            }
        }

        public void ImportObj()
        {
            Objs = new List<ObjVolume>();
            Objs.AddRange(ObjVolume.LoadFromFile(FilePath, Matrix4d.Identity));
        }

        public override void DrawNative(Color? clr = null)
        {

            foreach (var model in Objs)
            {
                var v = model;
                /*
                                var maxx = model.GetVerts().Max(x => x.X);
                                var minx = model.GetVerts().Min(x => x.X);
                                var maxy = model.GetVerts().Max(x => x.Y);
                                var miny = model.GetVerts().Min(x => x.Y);*/

                // GL.Enable(EnableCap.CullFace);
                //GL.CullFace(CullFaceMode.FrontAndBack);
                if (Name.Contains("window"))
                {
                    GL.Color4(Color.FromArgb(128, Color.White));
                }
                else
                {
                    GL.Color4(clr.Value);
                }

                int indiceat = 0;


                var t = v.faces;

                //     GL.Translate(mi.Position);
                // GL.Scale(mi.Scale);

                foreach (var tuple in t)
                {


                    var mater = tuple.Material;
                    if (mater != null)
                    {
                        if (Name.Contains("window"))
                        {
                            GL.Color4(mater.DiffuseColor.X, mater.DiffuseColor.Y, mater.DiffuseColor.Z, 0.5f);
                        }
                        else
                        {
                            GL.Color3(mater.DiffuseColor);
                        }
                        if (v.mat.textures.ContainsKey(mater.AmbientMap))
                        {


                            GL.Enable(EnableCap.Texture2D);
                            GL.BindTexture(TextureTarget.Texture2D, v.mat.textures[mater.AmbientMap]);

                        }
                        if (v.mat.textures.ContainsKey(mater.DiffuseMap))
                        {


                            GL.Enable(EnableCap.Texture2D);

                            GL.BindTexture(TextureTarget.Texture2D, v.mat.textures[mater.DiffuseMap]);

                        }
                    }


                    if (tuple.Vertexes.Count() == 3)
                    {
                        GL.Begin(PrimitiveType.Triangles);
                        GL.TexCoord2(tuple.Item1.TextureCoord);
                        GL.Normal3(tuple.Item1.Normal);
                        GL.Vertex3(tuple.Item1.Position);

                        GL.TexCoord2(tuple.Item2.TextureCoord);
                        GL.Normal3(tuple.Item2.Normal);
                        GL.Vertex3(tuple.Item2.Position);

                        GL.TexCoord2(tuple.Item3.TextureCoord);
                        GL.Normal3(tuple.Item3.Normal);
                        GL.Vertex3(tuple.Item3.Position);
                        GL.End();
                    }
                    if (tuple.Vertexes.Count() == 4)
                    {
                        GL.Begin(PrimitiveType.Quads);
                        GL.TexCoord2(tuple.Item1.TextureCoord);
                        GL.Normal3(tuple.Item1.Normal);
                        GL.Vertex3(tuple.Item1.Position);

                        GL.TexCoord2(tuple.Item2.TextureCoord);
                        GL.Normal3(tuple.Item2.Normal);
                        GL.Vertex3(tuple.Item2.Position);

                        GL.TexCoord2(tuple.Item3.TextureCoord);
                        GL.Normal3(tuple.Item3.Normal);
                        GL.Vertex3(tuple.Item3.Position);


                        GL.TexCoord2(tuple.Item4.TextureCoord);
                        GL.Normal3(tuple.Item4.Normal);
                        GL.Vertex3(tuple.Item4.Position);
                        GL.End();
                    }



                    GL.Disable(EnableCap.Texture2D);

                }
            }
        }
        public bool IsLoaded
        {
            get
            {
                return Objs != null;
            }
        }
        public override Vector3d GetBbox(Matrix4d? mtr = null)
        {
            if (mtr == null)
            {
                mtr = Matrix4d.Identity;
            }
            bool inited = false;

            if (oldbbox != mtr.Value)
            {
                BBoxDirty = true;
            }
            if (BBoxDirty)
            {
                oldbbox = mtr.Value;
                MinsBbox = new double[3];
                MaxsBbox = new double[3];
                foreach (var mitem in Objs)
                {
                    foreach (var item in mitem.faces)
                    {
                        var d = new DrawPolygon();
                        List<DrawVertex> dv = new List<DrawVertex>();
                        foreach (var vitem in item.Vertexes)
                        {
                            var pos = new Vector4d(vitem.Position, 1) * mtr.Value;
                            for (int i = 0; i < 3; i++)
                            {
                                if (!inited)
                                {
                                    MinsBbox[i] = pos[i];
                                    MaxsBbox[i] = pos[i];
                                }
                                MinsBbox[i] = Math.Min(MinsBbox[i], pos[i]);
                                MaxsBbox[i] = Math.Max(MaxsBbox[i], pos[i]);
                            }
                            inited = true;
                        }
                    }
                }
                BBoxDirty = false;

            }
            return new Vector3d(MaxsBbox[0] - MinsBbox[0], MaxsBbox[1] - MinsBbox[1], MaxsBbox[2] - MinsBbox[2]);
        }
    }
}
