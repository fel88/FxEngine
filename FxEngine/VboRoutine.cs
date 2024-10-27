using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FxEngine.Loaders.OBJ;

namespace FxEngine
{
    public class VboRoutine
    {
        public struct VertexVbo
        {
            public float X, Y, Z;
            public float nx, ny, nz;
            public VertexVbo(float x, float y, float z, float _nx, float _ny, float _nz)
            {
                X = x; Y = y; Z = z;
                nx = _nx;
                ny = _ny;
                nz = _nz;
            }
            
            public const int Stride = 6 * 4;
        }

        
        public int ID_VBO;
        
        public int ID_EBO;

        
        public int ID_EBO_Wireframe;

        
        public VertexVbo[] Vertices;
        public ObjVolume Volume;
        
        public uint[] Indices;
        public int IndicesWireframeLength;

        
        public void InitializeVBO(ObjVolume vol)
        {
            List<VertexVbo> v = new List<VertexVbo>();

            Vector3 center = Vector3.Zero;
            foreach (var vector3 in vol.vertices)
            {
                center += vector3;
                if (vector3.Z < 1)
                {

                }
            }
            var maxz = vol.vertices.Max(z => z.Z);
            var minz = vol.vertices.Min(z => z.Z);

            center /= vol.vertices.Count();
            
            List<uint> ind = new List<uint>();
            List<uint> indw = new List<uint>();
            uint cnt = 0;
            foreach (var faceItem2 in vol.faces)
            {
                if (faceItem2.Vertexes.Count() != 3)
                {

                }
                uint orig = cnt;
                indw.Add(orig);
                indw.Add(orig + 1);
                indw.Add(orig);
                indw.Add(orig + 2);
                indw.Add(orig + 1);
                indw.Add(orig + 2);

                foreach (var faceVertex in faceItem2.Vertexes)
                {
                    faceVertex.Position -= center;
                    //faceVertex.Normal *= -1;
                    v.Add(new VertexVbo(faceVertex.Position.X, faceVertex.Position.Y, faceVertex.Position.Z, faceVertex.Normal.X, faceVertex.Normal.X, faceVertex.Normal.X));

                    ind.Add(cnt++);
                }
            }
            
            vol.Position = center;

            Vertices = v.ToArray();
            

            Indices = ind.Select(z => (uint)z).ToArray();


            
            GL.GenBuffers(1, out ID_VBO);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID_VBO);
            
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * VertexVbo.Stride), Vertices, BufferUsageHint.StaticDraw);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            
            GL.GenBuffers(1, out ID_EBO);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO);
            
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(uint)), Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);


            #region wireframe buffers

            
            GL.GenBuffers(1, out ID_EBO_Wireframe);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO_Wireframe);
            
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indw.Count * sizeof(uint)), indw.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            #endregion
            vol.Position = center;

            vol.ID_EBO = ID_EBO;
            vol.ID_EBO_Wireframe = ID_EBO_Wireframe;
            IndicesWireframeLength = indw.Count;
            vol.IndiciesWireframeLength = indw.Count;
            vol.ID_VBO = ID_VBO;
            vol.Indices = Indices;

        }

        
        public void RenderVBO(bool isSelected, Vector3 position, Quaternion orientation, Vector3 offset, Color color)
        {

            GL.PushMatrix();

            
            GL.Translate(Stuff.ToVector3(position));
            Vector3 axis;
            float ang;
            var q = Stuff.ToQuaternion(orientation);
            q.ToAxisAngle(out axis, out ang);

            GL.Rotate((float)(ang * 180.0f / Math.PI), axis);

            GL.Translate(Stuff.ToVector3(offset));


            
            if (isSelected)
            {
                GL.Color4(1.0f, 0, 0, 0.5f);

            }
            else
            {
                
                GL.Color4(color.R, color.G, color.B, (byte)128);
            }

            if (isSelected)//bbox
            {
                var vv = Volume.GetVerts();
                
            }

            
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);


            
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID_VBO);
            
            GL.VertexPointer(3, VertexPointerType.Float, VertexVbo.Stride, 0);
            GL.NormalPointer(NormalPointerType.Float, VertexVbo.Stride, (IntPtr)(3 * sizeof(float)));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO);
            

            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);



            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.Disable(EnableCap.Blend);

            GL.PopMatrix();
        }

        public class ObjectOrientationPosition
        {
            public Vector3 position;
            public Quaternion orientation;
            public Vector3 offset;
        }
        public void RenderVBOMulti(bool isSelected, Vector3[] positions, Quaternion qn, Vector3 offset)
        {


            
            if (isSelected)
            {
                GL.Color4(1.0f, 0, 0, 0.5f);

            }
            else
            {
                GL.Color4(1.0f, 1.0f, 1.0f, 0.5f);
            }

            
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);


            
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID_VBO);
            
            GL.VertexPointer(3, VertexPointerType.Float, VertexVbo.Stride, 0);
            GL.NormalPointer(NormalPointerType.Float, VertexVbo.Stride, (IntPtr)(3 * sizeof(float)));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO);
            
            foreach (var objectOrientationPosition in positions)
            {


                GL.PushMatrix();

                
                GL.Translate(Stuff.ToVector3(objectOrientationPosition));
                Vector3 axis;
                float ang;
                var q = Stuff.ToQuaternion(qn);
                q.ToAxisAngle(out axis, out ang);

                GL.Rotate((float)(ang * 180.0f / Math.PI), axis);
                GL.Translate(Stuff.ToVector3(offset));
                GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

                GL.PopMatrix();
            }

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.Disable(EnableCap.Blend);

        }

        public void RenderWireframe(DrawingContext ctx, Vector3 position, Quaternion orientation, Vector3 offset)
        {
            GL.PushMatrix();

            
            GL.Translate(position);
            Vector3 axis;
            float ang;
            var q = Stuff.ToQuaternion(orientation);
            q.ToAxisAngle(out axis, out ang);

            GL.Rotate((float)(ang * 180.0f / Math.PI), axis);
            GL.Translate(Stuff.ToVector3(offset));

            
            GL.Color3(0.0f, 0.0f, 0.0f);
            GL.LineWidth(ctx.LineWidth);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID_VBO);
            
            GL.VertexPointer(3, VertexPointerType.Float, VertexVbo.Stride, 0);
            GL.NormalPointer(NormalPointerType.Float, VertexVbo.Stride, (IntPtr)(3 * sizeof(float)));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID_EBO_Wireframe);
            
            GL.DrawElements(BeginMode.Lines, (int)IndicesWireframeLength, DrawElementsType.UnsignedInt, 0);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);

            GL.PopMatrix();
        }
    }
}