using System.Linq;
using System.IO;
using System.Reflection;

namespace FxEngine
{
    public static class ResourceFile
    {
        public static string GetFileText(string name, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }             

            var nms = assembly.GetManifestResourceNames();
            string ret = "";
            var nfr = nms.First(z => z.ToLower().Contains(name.ToLower()));            
            name = nfr;
            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                ret = reader.ReadToEnd();
            }
            return ret;
        }
    }
}
