using FxEngine.Loaders.OBJ;
using FxEngine.Shaders;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void ImportObj()
        {
            Objs = new List<ObjVolume>();
            Objs.AddRange(ObjVolume.LoadFromFile(FilePath, Matrix4.Identity));
        }
        public bool IsLoaded
        {
            get
            {
                return Objs != null;
            }
        }
        public override Vector3 GetBbox(Matrix4? mtr = null)
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
