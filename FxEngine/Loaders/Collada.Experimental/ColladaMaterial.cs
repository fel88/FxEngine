using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaMaterial
    {
        public string Name;
        public string Id;
        public ColladaEffect Effect;

        public static ColladaMaterial Parse(XElement iitem, ColladaParseContext ctx)
        {
            ColladaMaterial ret = new ColladaMaterial();
            ret.Id = iitem.Attribute("id").Value;
            ret.Name = iitem.Attribute("name").Value;
            if (iitem.ToString().Contains("instance_effect"))
            {
                var fr = iitem.Descendants(XName.Get("instance_effect", ctx.Ns)).First();
                var val = fr.Attribute("url").Value.Replace("#", "");
                var eflib = ctx.Model.Libraries.OfType<ColladaEffectsLibrary>().First();
                var fref = eflib.Effects.First(z => z.Id == val);
                ret.Effect = fref;

            }
            else
            {
                var inp = iitem.Descendants(XName.Get("input", ctx.Ns)).First();
                var src = inp.Attribute("source").Value;
                var lib = ctx.Model.Libraries.OfType<ColladaTextureLibrary>().First();
                var frfr = lib.Textures.First(z => z.Id == src.Replace("#", ""));

            }
            return ret;
        }

        public void GlApply()
        {

            if (Effect != null)
            {
                if (Effect.Diffuse.IsColor)
                {
                    GL.Disable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.ColorMaterial);
                    GL.Color4(Effect.Diffuse.Color);
                }
                else
                {
                    GL.Enable(EnableCap.Texture2D);
                    GL.Disable(EnableCap.ColorMaterial);
                    GL.BindTexture(TextureTarget.Texture2D, Effect.Diffuse.Texture.TextureId);
                }
            }
        }
    }
}


