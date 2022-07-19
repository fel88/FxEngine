using FxEngine.Loaders.Collada;
using FxEngine.Loaders.OBJ;
using FxEngine.Shaders;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FxEngine
{
    public class ModelBlueprint
    {
        public DrawPolygon[] GetBoundingBoxModel(Matrix4 matrix)
        {

            List<DrawPolygon> dp = new List<DrawPolygon>();
            GetBbox();
            float[] mins = new float[3];
            float[] maxs = new float[3];
            List<FaceItem2> faces = new List<FaceItem2>();

            maxs = MaxsBbox;
            mins = MinsBbox;
            List<FaceVertex> vrt = new List<FaceVertex>();
            var v1 = new FaceVertex() { Position = new Vector3(mins[0], mins[1], mins[2]) };
            var v2 = new FaceVertex() { Position = new Vector3(maxs[0], mins[1], mins[2]) };
            var v3 = new FaceVertex() { Position = new Vector3(maxs[0], maxs[1], mins[2]) };
            var v4 = new FaceVertex() { Position = new Vector3(mins[0], maxs[1], mins[2]) };

            var v5 = new FaceVertex() { Position = new Vector3(mins[0], mins[1], maxs[2]) };
            var v6 = new FaceVertex() { Position = new Vector3(maxs[0], mins[1], maxs[2]) };
            var v7 = new FaceVertex() { Position = new Vector3(maxs[0], maxs[1], maxs[2]) };
            var v8 = new FaceVertex() { Position = new Vector3(mins[0], maxs[1], maxs[2]) };

            //bottom
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v2, v3 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v3, v4 } });
            //upper
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v5, v6, v7 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v5, v7, v8 } });
            //x-positive
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v2, v3, v6 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v3, v6, v7 } });
            //x-negative
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v4, v5 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v4, v5, v8 } });
            //y-positive
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v4, v3, v7 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v4, v7, v8 } });
            //y-negative
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v2, v6 } });
            faces.Add(new FaceItem2() { Vertexes = new FaceVertex[] { v1, v6, v5 } });

            foreach (var item in faces)
            {

                var d = new DrawPolygon();
                List<DrawVertex> dv = new List<DrawVertex>();
                foreach (var vitem in item.Vertexes)
                {

                    var trans1 = matrix.ExtractTranslation();
                    var v22 = Vector3.TransformVector(vitem.Position, matrix);
                    v22 += trans1;


                    dv.Add(new DrawVertex() { Position = v22 });
                }

                d.Vertices = dv.ToArray();
                dp.Add(d);


            }

            return dp.ToArray();
        }

        public int Id { get; set; }
        public string FilePath { get; set; }

        public bool IsLoaded
        {
            get
            {
                return Objs != null;
            }
        }
        public string Name { get; set; }
        public ModelBlueprint(string name, string path)
        {
            FilePath = path;
            Name = name;
        }
        public ModelBlueprint(string name)
        {

            Name = name;
        }
        public Matrix4 Matrix;

        public void ImportObj()
        {
            Objs = new List<ObjVolume>();
            Objs.AddRange(ObjVolume.LoadFromFile(FilePath, Matrix4.Identity));
        }

        public VaoModel VaoModel;

        public ModelBlueprint(string name, IEnumerable<ObjVolume> vols)
        {
            Name = name;
            Objs = new List<ObjVolume>();
            Objs.AddRange(vols);
            VaoModel = new VaoModel();
            VaoModel.ModelInit(Objs.ToArray());
            Vector3 res = new Vector3();
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

        public List<ObjVolume> Objs;
        public ColladaModel Model;

        public bool BBoxDirty = true;
        public float[] MinsBbox;
        public float[] MaxsBbox;

        Matrix4 oldbbox = Matrix4.Identity;
        public Vector3 GetBbox(Matrix4? mtr = null)
        {
            if (mtr == null)
            {
                mtr = Matrix4.Identity;
            }
            bool inited = false;

            if (oldbbox != mtr.Value)
            {
                BBoxDirty = true;
            }
            if (BBoxDirty)
            {
                oldbbox = mtr.Value;
                MinsBbox = new float[3];
                MaxsBbox = new float[3];
                foreach (var mitem in Objs)
                {
                    foreach (var item in mitem.faces)
                    {
                        var d = new DrawPolygon();
                        List<DrawVertex> dv = new List<DrawVertex>();
                        foreach (var vitem in item.Vertexes)
                        {
                            var pos = new Vector4(vitem.Position, 1) * mtr.Value;
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
            return new Vector3(MaxsBbox[0] - MinsBbox[0], MaxsBbox[1] - MinsBbox[1], MaxsBbox[2] - MinsBbox[2]);
        }
    }
}
