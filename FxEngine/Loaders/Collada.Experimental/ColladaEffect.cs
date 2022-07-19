using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaEffect
    {

        public string Id;

        public ColladaEffectChannelInfo Diffuse = new ColladaEffectChannelInfo() { Type = ColladaChannelInfoType.Diffuse };
        public ColladaEffectChannelInfo Emisson = new ColladaEffectChannelInfo() { Type = ColladaChannelInfoType.Emission };
        public ColladaEffectChannelInfo Ambient = new ColladaEffectChannelInfo() { Type = ColladaChannelInfoType.Ambient };
        public ColladaEffectChannelInfo Specular = new ColladaEffectChannelInfo() { Type = ColladaChannelInfoType.Specular };
        public ColladaEffectChannelInfo Transparent = new ColladaEffectChannelInfo() { Type = ColladaChannelInfoType.Transparent };
        public ColladaEffectChannelInfo[] Channels
        {
            get
            {
                List<ColladaEffectChannelInfo> ret = new List<ColladaEffectChannelInfo>();
                ret.Add(Diffuse);
                ret.Add(Emisson);
                ret.Add(Ambient);
                ret.Add(Specular);
                ret.Add(Transparent);
                return ret.ToArray();
            }
        }
        public float Shininess;

        public static ColladaEffect Parse(XElement elem, ColladaParseContext ctx)
        {
            ColladaEffect ef = new ColladaEffect();
            ef.Id = elem.Attribute("id").Value;

            foreach (var item in elem.Descendants(XName.Get("technique", ctx.Ns)))
            {

                foreach (var teitem in item.Elements())
                {
                    //lambert,phong
                    foreach (var eitem in teitem.Elements())
                    {
                        var val = eitem.Name.LocalName;
                        ColladaEffectChannelInfo info = null;
                        switch (val)
                        {
                            case "diffuse":
                                info = ef.Diffuse;
                                break;
                            case "ambient":
                                info = ef.Ambient;
                                break;
                            case "specular":
                                info = ef.Specular;
                                break;
                            case "emission":
                                info = ef.Emisson;
                                break;
                            case "transparent":
                                info = ef.Emisson;
                                break;
                        }
                        foreach (var eeitem in eitem.Elements())
                        {
                            if (eeitem.Name.LocalName == "texture")
                            {
                                var sampler = eeitem.Attribute("texture").Value;
                                var param1 = elem.Descendants().First(z => z.Attribute("sid") != null && z.Attribute("sid").Value == sampler);
                                var source = param1.Descendants().First(z => z.Name.LocalName == "source").Value;
                                var param2 = elem.Descendants().First(z => z.Attribute("sid") != null && z.Attribute("sid").Value == source);
                                var path = param2.Descendants().First(z => z.Name.LocalName == "init_from").Value;
                                var img = ctx.Model.ImagesLibrary.Images.First(z => z.Id == path || z.Name == path);
                                info.IsColor = false;

                                info.Texture = new ColladaTexture() { Image = img };
                            }

                            if (eeitem.Name.LocalName == "color")
                            {
                                var ar = eeitem.Value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(z => float.Parse(z, CultureInfo.InvariantCulture)).Select(z => (int)(z * 255)).ToArray();
                                info.Color = Color.FromArgb(ar[3], ar[0], ar[1], ar[2]);
                            }
                        }
                    }
                }
            }
            return ef;
        }
    }
}


