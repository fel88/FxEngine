using OpenTK.Graphics.OpenGL;

namespace FxEngine.Gui
{
    public class VertexArray
    {
        uint RectVBO;
        uint RectVAO;

        public void Create(float[] data)
        {
            GL.GenVertexArrays(1, out RectVAO);


            GL.GenBuffers(1, out RectVBO);


            GL.BindVertexArray(RectVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, RectVBO);

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);
            count = data.Length / 3;

        }
        int count = 0;
        internal void DrawAllTriangles()
        {
            GL.BindVertexArray(RectVAO);

            //GL.DrawElements(PrimitiveType.Quads, 1, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, count);

        }
        internal void DrawAllLineLoop()
        {
            GL.BindVertexArray(RectVAO);

            //GL.DrawElements(PrimitiveType.Quads, 1, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, count);
        }

        internal void DrawAllLinesStrip()
        {
            GL.BindVertexArray(RectVAO);

            //GL.DrawElements(PrimitiveType.Quads, 1, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, count);
        }
    }
}
