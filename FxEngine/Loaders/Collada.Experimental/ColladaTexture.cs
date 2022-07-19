using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaTexture
    {
        public string Id;
        public string Name;
        public ColladaImage Image;

        #region GL section
        public int TextureId;

        public bool GlInited = false;
        public void InitGlTexture()
        {
            if (GlInited) return;

            GlInited = true;
            var bmpt = Image.Bitmap;
            TextureId = GL.GenTexture();
            var pf = bmpt.PixelFormat;

            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            var data = bmpt.LockBits(new System.Drawing.Rectangle(0, 0, bmpt.Width, bmpt.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmpt.PixelFormat);
            
            var format = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
            var sz = System.Drawing.Image.GetPixelFormatSize(bmpt.PixelFormat);
            if (sz == 24)
            {
                format = PixelFormat.Bgr;
            }
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpt.Width, bmpt.Height, 0,
            format
            , PixelType.UnsignedByte, data.Scan0);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            bmpt.UnlockBits(data);
        }

        #endregion

        public static ColladaTexture Parse(XElement iitem, ColladaParseContext ctx)
        {
            ColladaTexture ret = new ColladaTexture();
            var sid = iitem.Attribute("id").Value;
            var name = iitem.Attribute("name").Value;

            var fr = iitem.Descendants(XName.Get("input", ctx.Ns)).First();
            var va = fr.Attribute("source").Value;
            var frl = ctx.Model.Libraries.OfType<ColladaImageLibrary>().First();
            var ff = frl.Images.First(z => z.Id == va.Replace("#", ""));
            ret.Image = ff;
            ret.Id = sid;
            ret.Name = name;


            return ret;
        }
    }
}


