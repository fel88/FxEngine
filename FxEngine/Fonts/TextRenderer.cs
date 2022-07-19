using System.Collections.Generic;
using System.Drawing;
using FxEngine.Shaders;
using OpenTK.Graphics.OpenGL;

namespace FxEngine.Fonts
{
    public class TextRenderer
    {
        public virtual void DrawText(string text, PointF pos)
        {
            //InitCharTextures();
            if (string.IsNullOrEmpty(text)) return;
            var rgs = kr.GetStringRegions(text);
            if (shader != null)
            {
                shader.Use();
                //(shader as SdfShader).txtr=(fr.Texture);
                shader.AtlasTextureId = AtlasTexture;
                shader.SetTexture();
            }
            var isdep = GL.GetBoolean(GetPName.DepthTest);
            var isblend = GL.GetBoolean(GetPName.Blend);
            GL.Disable(EnableCap.DepthTest);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            for (int i = 0; i < text.Length; i++)
            {
                var rg = rgs[i];
                RenderChar(text[i], new PointF(rg.DrawPoint.X + pos.X, rg.DrawPoint.Y + pos.Y));
            }

            GL.UseProgram(0);
            if (isdep)
            {
                GL.Enable(EnableCap.DepthTest);
            }
            if (!isblend)
            {
                GL.Disable(EnableCap.Blend);
            }
        }
        public virtual void RenderChar(char c, PointF pos)
        {

        }
        public KerningRoutine kr;

        public virtual ITextShader shader { get; set; }
        public Atlas atlas = new Atlas();
        public Dictionary<char, CharTextureInfo> infos = new Dictionary<char, CharTextureInfo>();
        bool charTexturesInited = false;

        public int AtlasTexture
        {
            get
            {
                return shader.AtlasTextureId;
            }
            set
            {
                shader.AtlasTextureId = value;
            }
        }

        public void InitCharTextures(float zoom = 1)
        {
            if (charTexturesInited) return;
            charTexturesInited = true;

            #region atlas texture create
            AtlasTexture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, AtlasTexture);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            var bmpt0 = atlas.AtlasBitmap;

            //var vb = InitCharQuad(bmpt.Width * zoom, bmpt.Height * zoom);

            var data0 = bmpt0.LockBits(new System.Drawing.Rectangle(0, 0, bmpt0.Width, bmpt0.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmpt0.PixelFormat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpt0.Width, bmpt0.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra
            , PixelType.UnsignedByte, data0.Scan0);
            //GL.GenerateTextureMipmap(AtlasTexture);
            bmpt0.UnlockBits(data0);

            #endregion
            foreach (var item in atlas.Chars)
            {
                var px = item.Bound.Left / (float)atlas.AtlasBitmap.Width;
                var py = item.Bound.Top / (float)atlas.AtlasBitmap.Height;
                var sx = item.Bound.Size.Width / (float)atlas.AtlasBitmap.Width;
                var sy = item.Bound.Size.Height / (float)atlas.AtlasBitmap.Height;
                var vb = InitCharQuad(item.Bound.Size.Width * zoom, item.Bound.Size.Height * zoom, new RectangleF(px, py, sx, sy));

                infos.Add((char)item.Index, new CharTextureInfo(item)
                {
                    //Texture = txtr,
                    VertexBuffer = vb,
                    Size = item.Bound.Size
                });
            }
        }

        public float koef = 20;

        public static int InitCharQuad(float w = 50, float h = 50, RectangleF? textrs = null)
        {
            List<float> fls = new List<float>();

            if (Helpers.IsCompatibilityMode())
            {
                if (textrs != null)
                {
                    var r = textrs.Value;
                    fls.AddRange(new float[] { 0, 0, 0, r.Left, r.Bottom });
                    fls.AddRange(new float[] { w, 0, 0, r.Right, r.Bottom });
                    //fls.AddRange(new float[] { 0, h, 0, r.Left, r.Bottom });

                    fls.AddRange(new float[] { w, h, 0, r.Right, r.Top });
                    //    fls.AddRange(new float[] { w, 0, 0, r.Right, r.Top });
                    fls.AddRange(new float[] { 0, h, 0, r.Left, r.Top });
                }
            }
            else
            {
                bool oldStyle = false;
                if (oldStyle)
                {
                    if (textrs != null)
                    {
                        var r = textrs.Value;
                        fls.AddRange(new float[] { 0, 0, 0, r.Left, r.Bottom });
                        fls.AddRange(new float[] { w, 0, 0, r.Right, r.Bottom });
                        fls.AddRange(new float[] { 0, h, 0, r.Left, r.Top });

                        fls.AddRange(new float[] { w, h, 0, r.Right, r.Top });
                        fls.AddRange(new float[] { w, 0, 0, r.Right, r.Bottom });
                        fls.AddRange(new float[] { 0, h, 0, r.Left, r.Top });
                    }
                }
                else
                {
                    if (textrs != null)
                    {
                        var r = textrs.Value;
                        fls.AddRange(new float[] { 0, 0, 0, r.Left, r.Top });
                        fls.AddRange(new float[] { w, 0, 0, r.Right, r.Top });
                        fls.AddRange(new float[] { 0, -h, 0, r.Left, r.Bottom });

                        fls.AddRange(new float[] { w, -h, 0, r.Right, r.Bottom });
                        fls.AddRange(new float[] { w, 0, 0, r.Right, r.Top });
                        fls.AddRange(new float[] { 0, -h, 0, r.Left, r.Bottom });
                    }
                }
            }
            var vertices = fls.ToArray();

            int ret;
            int VAO;
            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out ret);


            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ret);

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, ret);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            return VAO;
        }

        public Font Font;
        public virtual void Init(Graphics gr)
        {


        }
    }
}
