using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace FxEngine.Gui
{
    public class NativeDrawProvider
    {
        BaseGlDrawingContext Context;
        public void Reset()
        {
            ShiftX = 0;
            ShiftY = 0;
            Color = new Vector4();
            CurrentBound = new GuiBounds(0, 0, Context.Width, Context.Height);
        }

        public NativeDrawProvider(BaseGlDrawingContext dc)
        {
            Context = dc;
            Reset();
            GenerateRectVbo();
        }

        public Vector4 Color;

        public void SetColor(float x, float y, float z)
        {
            Color = new Vector4(x, y, z, 1);
            GL.Color3(x, y, z);
        }

        public void SetColor(int x, int y, int z)
        {
            //Color = new Vector4(x, y, z, 1);
            GL.Color3(System.Drawing.Color.FromArgb(x, y, z));
        }

        VertexArray rect1 = new VertexArray();
        VertexTextureArray rect3 = new VertexTextureArray();
        VertexArray rect2 = new VertexArray();

        public void GenerateRectVbo()
        {

            float[] vertices = new float[]{

        0,0, 0,
     1f,0,0,
     1f,-1f,0,

      0,0, 0,
     1f,-1f,0,
     0,-1f,0,
};

            float[] vertices2 = new float[]{
        0,0, 0,
     1f,0,0,
     1f,-1f,0,
     0,-1f,0,
};
            float[] vertices3 = new float[]{
        0,0, 0,0,0,
        1f,-1f,0,1,1,
     1f,0,0,1,0,


      0,0, 0,0,0,
     1f,-1f,0,1,1,
     0,-1f,0,0,1
};

            rect1.Create(vertices);
            rect2.Create(vertices2);
            rect3.Create(vertices3);

            RectShader = new RectShader();
            RectShader.Init();

            RectTextureShader = new RectTextureShader();
            RectTextureShader.Init();
        }

        public void GetError()
        {
            var er = GL.GetError();
            if (er != ErrorCode.NoError)
            {

            }
        }

        public RectShader RectShader;
        public RectTextureShader RectTextureShader;

        public void FillRectangle(GuiBounds b)
        {
            var x = b.Left;
            var y = b.Top;
            var w = b.Width;
            var h = b.Height;
            GL.Begin(PrimitiveType.Quads);
            Vertex(x, y);
            Vertex(x + w, y);
            Vertex(x + w, y + h);
            Vertex(x, y + h);
            GL.End();
        }

        public void FillRectangleCore(GuiBounds b)
        {
            RectShader.Use();
            RectShader.SetUniformsData();
            GetError();

            rect1.DrawAllTriangles();

        }

        public void TextureRectangle(GuiBounds b)
        {
            GetError();
            rect3.DrawAllTriangles();
        }

        public void DrawRectangle(float x, float y, float w, float h)
        {
            GL.Begin(PrimitiveType.LineLoop);
            Vertex(x, y);
            Vertex(x + w, y);
            Vertex(x + w, y + h);
            Vertex(x, y + h);
            GL.End();
        }

        public void DrawRectangleCore(float x, float y, float w, float h)
        {
            RectShader.Use();
            RectShader.SetUniformsData();
            GetError();

            rect2.DrawAllLineLoop();
        }

        public void FillTriangleCore(float x, float y, float w, float h)
        {
            return;
        }

        public void FillTriangle(float x, float y, float w, float h)
        {
            GL.Begin(PrimitiveType.Triangles);
            Vertex(x, y);
            Vertex(x + w, y);
            Vertex(x + w / 2, y + h);
            GL.End();
        }

        public void TranslateAccum(float x, float y)
        {
            ShiftX = x;
            ShiftY = y;
        }

        public void Translate(float x, float y)
        {
            GL.Translate(x, Context.Height - y, 0);
            //ShiftX = x;
            // ShiftY = y;
        }

        public float ShiftX;
        public float ShiftY;

        public GuiBounds CurrentBound { get; internal set; }

        public void Vertex(float x, float y)
        {
            var y1 = Context.Height - y;
            GL.Vertex3(x + ShiftX, y1 + ShiftY, 0);
        }

        internal void DrawRectangle(GuiBounds rect)
        {
            DrawRectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        }
        internal void DrawRectangleCore(GuiBounds rect)
        {
            DrawRectangleCore(rect.Left, rect.Top, rect.Width, rect.Height);
        }
        internal void SetColor(byte v1, byte v2, byte v3, byte v4)
        {
            GL.Color4(v1, v2, v3, v4);
        }
    }
}
