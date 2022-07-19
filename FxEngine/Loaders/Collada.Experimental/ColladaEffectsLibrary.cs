using System.Collections.Generic;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaEffectsLibrary : ColladaLibrary
    {

        public override void InitGl()
        {
            foreach (var item in Effects)
            {
                foreach (var citem in item.Channels)
                {
                    if (citem.Texture != null)
                    {
                        citem.Texture.InitGlTexture();
                    }
                }
            }
        }
        public List<ColladaEffect> Effects = new List<ColladaEffect>();
        public static ColladaEffectsLibrary Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaEffectsLibrary ret = new ColladaEffectsLibrary();
            foreach (var item in elem.Descendants(XName.Get("effect", ctx.Ns)))
            {
                ret.Effects.Add(ColladaEffect.Parse(item, ctx));
            }
            return ret;
        }
    }
}


