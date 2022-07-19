using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaParseContext
    {
        public IDataProvider DataProvider;
        public XName GetXmlName(string nm)
        {
            return XName.Get(nm, Ns);
        }
        public ColladaModel Model;
        public ColladaScene CurrentScene;

        public string Ns;

        public float Dpi { get; set; }
    }
}


