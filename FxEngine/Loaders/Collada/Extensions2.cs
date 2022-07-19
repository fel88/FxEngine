using System.Linq;
using System.Xml.Linq;

namespace FxEngine.Loaders.Collada
{
    public static class Extensions2
    {
        public static XElement getChildWithAttribute(this XElement e, string name, string aname, string aval)
        {
            return e.Elements().First(z => z.Name.LocalName == name && z.Attribute(aname) != null && z.Attribute(aname).Value == aval);
        }
    }
}


