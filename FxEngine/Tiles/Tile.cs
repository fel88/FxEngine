using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;

namespace FxEngine.Tiles
{
    public class Tile
    {
        public static int NewId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path;

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF(Position.X, Position.Y, bmpt.Width, bmpt.Height);
            }
        }

        public void Init(string path, IDataProvider dp = null)
        {
            if (dp == null) { dp = new PhysicalFilesystemDataProvider(); }
            Path = path;
            if (bmpt != null)
            {
                DeleteTexture();
            }

            texture = GL.GenTexture();
            bmpt = dp.GetBitmap(path);

            if (ForceSizePreLoad)
            {
                var b2 = new Bitmap(Width, Height);
                var gr = Graphics.FromImage(b2);
                gr.DrawImage(bmpt, new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, bmpt.Width, bmpt.Height), GraphicsUnit.Pixel);
                bmpt.Dispose();
                bmpt = b2;
                gr.Dispose();
            }
        }

        public int Width;
        public bool ForceSizePreLoad;
        public int Height;


        public void DeleteTexture()
        {
            GL.DeleteTexture(texture);
        }
        public int texture;
        public Bitmap bmpt;


        public Color BackColor { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.White;
        public float BorderWidth { get; set; } = 0;
        public int BackOpacity { get; set; } = 255;
        public bool RightAlign = false;
        public bool Vertical = false;

        public SizeF Measure;

        public string Text;
        public List<string> MultiLines = new List<string>();
        public float MultiLineYOffset = 25;
        public Font Font = new Font("Arial", 12, FontStyle.Regular);
        public Brush Brush = Brushes.Black;
        public PointF Position;
        public int Opacity { get; set; } = 255;
        public float Scale { get; set; } = 1;

        public float Z = 1;
        public bool IsBindTextureRequired = true;
        public Size? ForceQuadSize;
        public float TexturesRepeatsS = 1;
        public float TexturesRepeatsW = 1;

        public Matrix4 Orientation = Matrix4.Identity;
        public void DrawQuad()
        {
            GL.PushMatrix();
            int[] vw = new int[4];
            GL.GetInteger(GetPName.Viewport, vw);

            int gap = 0;

            GL.Translate(Position.X - gap, Position.Y - gap, 0);
            GL.MultMatrix(ref Orientation);

            GL.Scale(Scale, Scale, Scale);
            GL.Enable(EnableCap.Texture2D);
            if (IsBindTextureRequired)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture);
            }




            GL.Begin(PrimitiveType.Quads);

            var realWidth = bmpt.Width;
            var realHeight = bmpt.Height;
            if (ForceQuadSize != null)
            {
                realWidth = ForceQuadSize.Value.Width;
                realHeight = ForceQuadSize.Value.Height;
            }
            realWidth += gap * 2;
            realHeight += gap * 2;
            GL.TexCoord3(TexturesRepeatsS, -TexturesRepeatsW, 0f); GL.Vertex3(realWidth, realHeight, Z);
            GL.TexCoord3(0.0f, -TexturesRepeatsW, 0f); GL.Vertex3(0f, realHeight, Z);
            GL.TexCoord3(0.0f, 0.0f, 0f); GL.Vertex3(0f, 0f, Z);
            GL.TexCoord3(TexturesRepeatsS, 0.0f, 0f); GL.Vertex3(realWidth, 0f, Z);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            //GL.Color3(Color.White);
            GL.PopMatrix();
            if (Wireframe)
            {
                DrawQuadWire();
            }
        }
        public bool Wireframe = false;
        public void DrawQuadWire()
        {
            GL.PushMatrix();
            int[] vw = new int[4];
            GL.GetInteger(GetPName.Viewport, vw);

            int gap = 0;
            GL.Translate(Position.X - gap, Position.Y - gap, 0);
            GL.Scale(Scale, Scale, Scale);
            GL.Disable(EnableCap.Texture2D);
            GL.Color3(Color.Black);
            GL.LineWidth(2);




            GL.Begin(PrimitiveType.Lines);

            var realWidth = bmpt.Width;
            var realHeight = bmpt.Height;
            if (ForceQuadSize != null)
            {
                realWidth = ForceQuadSize.Value.Width;
                realHeight = ForceQuadSize.Value.Height;
            }
            realWidth += gap * 2;
            realHeight += gap * 2;
            GL.Vertex3(realWidth, realHeight, Z);
            GL.Vertex3(0f, realHeight, Z);
            GL.Vertex3(0f, realHeight, Z);
            GL.Vertex3(0f, 0f, Z);
            GL.Vertex3(0f, 0f, Z);
            GL.Vertex3(realWidth, 0f, Z);
            GL.Vertex3(realWidth, 0f, Z);
            GL.Vertex3(realWidth, realHeight, Z);

            GL.End();


            GL.PopMatrix();
            GL.LineWidth(1);

        }

        public bool Visible = true;

        public void StoreData()
        {


            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            var data = bmpt.LockBits(new System.Drawing.Rectangle(0, 0, bmpt.Width, bmpt.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmpt.PixelFormat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpt.Width, bmpt.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra
            , PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            bmpt.UnlockBits(data);
            first = true;
            GL.Disable(EnableCap.Texture2D);
        }
        public bool first = true;
        public Color MainColor = Color.White;
        public bool IsForceSize = false;
        public Size ForceSize;
        public bool BlendFunc1 = false;


        public void CopyData()
        {

            Size sz = new Size(bmpt.Width, bmpt.Height);
            if (IsForceSize)
            {
                sz = new Size(ForceSize.Width, ForceSize.Height);
                var bmp = new Bitmap(sz.Width, sz.Height, bmpt.PixelFormat);
                var gr = Graphics.FromImage(bmp);
                gr.DrawImage(bmpt, new Rectangle(0, 0, sz.Width, sz.Height), new Rectangle(0, 0, bmpt.Width, bmpt.Height), GraphicsUnit.Pixel);
                bmpt = bmp;
            }
            var data = bmpt.LockBits(new System.Drawing.Rectangle(0, 0, sz.Width, sz.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmpt.PixelFormat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sz.Width, sz.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            bmpt.UnlockBits(data);
        }

        public void Draw()
        {
            if (!Visible) return;
            GL.Color4(Color.FromArgb(Opacity, MainColor));
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            if (BlendFunc1)
            {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
            if (IsBindTextureRequired)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            if (first)
            {
                first = false;
                CopyData();
            }
            DrawQuad();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }
    }
}
