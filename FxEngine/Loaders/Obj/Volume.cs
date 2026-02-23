using OpenTK;
using OpenTK.Mathematics;

namespace FxEngine.Loaders.OBJ
{    
    public abstract class Volume
    {
        public Vector3d Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public virtual int VertCount { get; set; }
        public virtual int IndiceCount { get; set; }
        public virtual int ColorDataCount { get; set; }
        public virtual int NormalCount { get { return Normals.Length; } }

        public Matrix4d ModelMatrix = Matrix4d.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        Vector3d[] Normals = new Vector3d[0];

        public abstract Vector3d[] GetVerts();
        public abstract int[] GetIndices(int offset = 0);
        public abstract Vector3[] GetColorData();
        public abstract void CalculateModelMatrix();

        public bool IsTextured = false;
        public int TextureID;
        public int TextureCoordsCount;
        public abstract Vector2d[] GetTextureCoords();

        public virtual Vector3d[] GetNormals()
        {
            return Normals;
        }

        public void CalculateNormals()
        {
            Vector3d[] normals = new Vector3d[VertCount];
            Vector3d[] verts = GetVerts();
            int[] inds = GetIndices();
            
            for (int i = 0; i < IndiceCount; i += 3)
            {
                Vector3d v1 = verts[inds[i]];
                Vector3d v2 = verts[inds[i + 1]];
                Vector3d v3 = verts[inds[i + 2]];
                                
                normals[inds[i]] += Vector3d.Cross(v2 - v1, v3 - v1);
                normals[inds[i + 1]] += Vector3d.Cross(v2 - v1, v3 - v1);
                normals[inds[i + 2]] += Vector3d.Cross(v2 - v1, v3 - v1);
            }

            for (int i = 0; i < NormalCount; i++)
            {
                normals[i] = normals[i].Normalized();
            }

            Normals = normals;
        }
    }
}