using OpenTK.Mathematics;
using System.Collections.Generic;

namespace FxEngine
{
    public class PreloadedMeshArray
    {
        public readonly float[] Data;
        public readonly int TrianglesQty;
        public readonly bool WithNormals;
        public PreloadedMeshArray(IReadOnlyList<Vector3d> verts)
        {
            WithNormals = false;
            int idx = 0;
            Data = new float[verts.Count * 3];
            for (int i = 0; i < verts.Count; i++)
            {
                Data[idx++] = (float)verts[i].X;
                Data[idx++] = (float)verts[i].Y;
                Data[idx++] = (float)verts[i].Z;
            }

            TrianglesQty = verts.Count;
        }

        public PreloadedMeshArray(IReadOnlyList<Vector3d> verts, IReadOnlyList<Vector3d> normals)
        {
            WithNormals = true;
            int idx = 0;
            Data = new float[verts.Count * 3 * 2];
            for (int i = 0; i < verts.Count; i++)
            {
                Data[idx++] = (float)verts[i].X;
                Data[idx++] = (float)verts[i].Y;
                Data[idx++] = (float)verts[i].Z;

                Data[idx++] = (float)normals[i].X;
                Data[idx++] = (float)normals[i].Y;
                Data[idx++] = (float)normals[i].Z;
            }

            TrianglesQty = verts.Count;
        }
    }
}

