using System.Collections.Generic;
using System.Drawing;
using FxEngine.Fonts.SDF;
using OpenTK.Graphics.OpenGL;

namespace FxEngine
{
    public class GlLabel
    {
        public void InitF(float w = 500, float h = 30, bool antiAlias = false)
        {
            Init((int)w, (int)h, antiAlias);
        }

        public void Init(int w = 500, int h = 30, bool antiAlias = false)
        {
            if (bmpt != null)
            {
                DeleteTexture();
            }
            bmpt = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gfx = Graphics.FromImage(bmpt);
            if (antiAlias)
            {
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }

            texture = GL.GenTexture();
        }

        public void DeleteTexture()
        {
            GL.DeleteTexture(texture);
        }
        int texture;
        public Bitmap bmpt;
        Graphics gfx;

        public Color BackColor { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.White;
        public float BorderWidth { get; set; } = 0;
        public int BackOpacity { get; set; } = 255;
        public bool RightAlign = false;
        public bool Vertical = false;

        public SizeF Measure;
        public void Update()
        {
            gfx.ResetTransform();
            gfx.Clear(Color.FromArgb(BackOpacity, BackColor));
            //gfx.Clear(BackColor);
            var ms = gfx.MeasureString(Text, Font);
            Measure = ms;
            if (Vertical)
            {
                gfx.TranslateTransform(-ms.Height / 2 + bmpt.Width / 2, ms.Width + bmpt.Height / 2 - ms.Width / 2);
                gfx.RotateTransform(-90);
            }
            if (RightAlign)
            {
                gfx.DrawString(Text, Font, Brush, new PointF(bmpt.Width - ms.Width, 0));
            }
            else
            {
                gfx.DrawString(Text, Font, Brush, new PointF(0, 0));

            }
            int cnt = 1;
            foreach (var item in MultiLines)
            {
                gfx.DrawString(item, Font, Brush, new PointF(0, cnt * MultiLineYOffset));
                cnt++;
            }
            if (BorderWidth > 0)
            {
                gfx.DrawRectangle(new Pen(BorderColor, BorderWidth), 0, 0, bmpt.Width, bmpt.Height);
            }            
        }

        public string Text;
        public List<string> MultiLines = new List<string>();
        public float MultiLineYOffset = 25;
        public Font Font = new Font("Arial", 12, FontStyle.Regular);
        public Brush Brush = Brushes.Black;
        public PointF Position;
        public int Opacity { get; set; } = 128;
        public float Z = 1;

        public void DrawQuad()
        {
            GL.PushMatrix();
            int[] vw = new int[4];
            GL.GetInteger(GetPName.Viewport, vw);
            
            GL.Translate(Position.X, Position.Y, 0);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.Begin(PrimitiveType.Quads);
            var realWidth = bmpt.Width;
            var realHeight = bmpt.Height;

            GL.TexCoord3(1.0f, -1.0f, 0f); GL.Vertex3(realWidth, realHeight, Z);
            GL.TexCoord3(0.0f, -1.0f, 0f); GL.Vertex3(0f, realHeight, Z);
            GL.TexCoord3(0.0f, 0.0f, 0f); GL.Vertex3(0f, 0f, Z);
            GL.TexCoord3(1.0f, 0.0f, 0f); GL.Vertex3(realWidth, 0f, Z);

            GL.End();

            GL.Disable(EnableCap.Texture2D);            
            GL.PopMatrix();
        }

        public bool Visible = true;

        public void Draw(SdfTextRoutine sdf)
        {
            if (!Visible) return;

            GL.PushMatrix();
            if (!string.IsNullOrEmpty(Text))
            {
                sdf.DrawText(Text, Position);
                for (int i = 0; i < MultiLines.Count; i++)
                {
                    var str = MultiLines[i];
                    sdf.DrawText(str, new PointF(Position.X, Position.Y - (MultiLineYOffset * (i + 1))));
                }
            }
            GL.PopMatrix();
        }

        public void Draw()
        {
            if (!Visible) return;

            GL.Color4(Color.FromArgb(Opacity, Color.White));
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            var data = bmpt.LockBits(new System.Drawing.Rectangle(0, 0, bmpt.Width, bmpt.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmpt.PixelFormat);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpt.Width, bmpt.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra
            , PixelType.UnsignedByte, data.Scan0);
            
            bmpt.UnlockBits(data);
            DrawQuad();

            GL.Disable(EnableCap.Blend);
        }
    }
}
