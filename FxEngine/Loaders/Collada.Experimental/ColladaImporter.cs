using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaImporter
    {

        public static ColladaModel Load(string path, IDataProvider dp)
        {
            ColladaModel ret = new ColladaModel();


            ColladaParseContext ctx = new ColladaParseContext();
            ctx.DataProvider = dp;
            ctx.Dpi = 96;
            ret.FilePath = path;
            ctx.Model = ret;

            var doc = dp.LoadXml(path);
            var fr = doc.Descendants().First();
            var v = fr.Attribute("xmlns").Value;

            string ns = "http://www.collada.org/2005/11/COLLADASchema";
            ns = v;
            ctx.Ns = ns;


            #region geometry loader

            //GeometryLoader g = new GeometryLoader(doc.Descendants(XName.Get("library_geometries", ns)).First(), null, ctx);
            // MeshData meshData = g.extractModelData(ctx);

            //ret.Data = meshData;
            #endregion


            foreach (var item in doc.Descendants(XName.Get("library", ns)))
            {
                var tp = item.Attribute("type").Value;

                if (tp == "IMAGE")
                {
                    ret.Libraries.Add(ColladaImageLibrary.Parse(item, ctx));
                }
                if (tp == "TEXTURE")
                {
                    ret.Libraries.Add(ColladaTextureLibrary.Parse(item, ctx));
                }
                if (tp == "MATERIAL")
                {
                    ret.Libraries.Add(ColladaMaterialLibrary.Parse(item, ctx));
                }

            }
            foreach (var item in doc.Descendants(XName.Get("library_images", ns)))
            {
                ret.Libraries.Add(ColladaImageLibrary.Parse(item, ctx));
            }

            foreach (var item in doc.Descendants(XName.Get("library_effects", ns)))
            {
                ret.Libraries.Add(ColladaEffectsLibrary.Parse(item, ctx));
            }

            foreach (var item in doc.Descendants(XName.Get("library_materials", ns)))
            {
                ret.Libraries.Add(ColladaMaterialLibrary.Parse(item, ctx));
            }
            foreach (var item in doc.Descendants(XName.Get("library_geometries", ns)))
            {
                ret.Libraries.Add(ColladaGeometryLibrary.Parse(item, ctx));
            }
            foreach (var item in doc.Descendants(XName.Get("library_controllers", ns)))
            {
                ret.Libraries.Add(ColladaControllersLibrary.Parse(item, ctx));
            }
            foreach (var item in doc.Descendants(XName.Get("library_animations", ns)))
            {
                ret.Libraries.Add(ColladaAnimationsLibrary.Parse(item, ctx));
            }
            foreach (var item in doc.Descendants(XName.Get("visual_scene", ns)))
            {
                ret.Scenes.Add(ColladaScene.Parse(item, ctx));

            }


            var scene = doc.Descendants(XName.Get("scene", ns)).First();
            foreach (var item in scene.Descendants(XName.Get("node", ns)))
            {
                var id = item.Attribute("id").Value;
                var nm = item.Attribute("name").Value;
            }




            var cntrl = ret.Scenes.First().Nodes.FirstOrDefault(z => z.Controller != null);
            if (cntrl != null)
            {
                cntrl.Controller.Animation = AnimationLoader.loadAnimation(path, ctx);
            }


            return ret;
        }
    }
}


