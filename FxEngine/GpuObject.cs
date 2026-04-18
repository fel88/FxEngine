using FxEngine.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace FxEngine
{
    public class GpuObject : IDisposable, IGpuObject
    {
        bool deleted = false;

        int numTriangles;
        int VBO, VAO;
        public GpuObject(IReadOnlyList<Vector3d> verts, IReadOnlyList<Vector3d> normals)
        {
            int idx = 0;
            float[] vertices = new float[verts.Count * 3 * 2];
            for (int i = 0; i < verts.Count; i++)
            {
                vertices[idx++] = (float)verts[i].X;
                vertices[idx++] = (float)verts[i].Y;
                vertices[idx++] = (float)verts[i].Z;

                vertices[idx++] = (float)normals[i].X;
                vertices[idx++] = (float)normals[i].Y;
                vertices[idx++] = (float)normals[i].Z;
            }

            numTriangles = verts.Count;

            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }

        public GpuObject(IReadOnlyList<Vector3d> verts)
        {
            int idx = 0;
            float[] vertices = new float[verts.Count * 3];
            for (int i = 0; i < verts.Count; i++)
            {
                vertices[idx++] = (float)verts[i].X;
                vertices[idx++] = (float)verts[i].Y;
                vertices[idx++] = (float)verts[i].Z;
            }

            numTriangles = verts.Count;

            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numTriangles);
            GL.BindVertexArray(0);

        }

        public void Dispose()
        {
            if (deleted)
                return;

            deleted = true;
            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
        }
    }
}

