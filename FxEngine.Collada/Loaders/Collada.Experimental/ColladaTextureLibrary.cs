using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaTextureLibrary : ColladaLibrary
    {

        public bool Inited = false;
        public override void InitGl()
        {
            if (Inited) return;
            Inited = true;
            foreach (var item in Textures)
            {
                item.InitGlTexture();
            }
        }

        public List<ColladaTexture> Textures = new List<ColladaTexture>();

        public static ColladaLibrary Parse(XElement item, ColladaParseContext ctx)
        {
            ColladaTextureLibrary ret = new ColladaTextureLibrary();
            foreach (var iitem in item.Descendants(XName.Get("texture", ctx.Ns)))
            {
                ret.Textures.Add(ColladaTexture.Parse(iitem, ctx));
            }
            return ret;
        }
    }
}


