using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace FxEngine
{
    public class PhysicalFilesystemDataProvider : IDataProvider
    {
        public Bitmap GetBitmap(string path)
        {
            return Bitmap.FromFile(path) as Bitmap;
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public byte[] GetFile(string path)
        {
            return File.ReadAllBytes(path);
        }

        public string GetFileAsString(string path)
        {
            return File.ReadAllText(path);
        }

        public XDocument LoadXml(string path)
        {
            return XDocument.Load(path);
        }
    }
}

