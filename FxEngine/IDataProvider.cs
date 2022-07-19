using System.Drawing;
using System.Xml.Linq;

namespace FxEngine
{
    public interface IDataProvider
    {
        Bitmap GetBitmap(string path);
        byte[] GetFile(string path);
        string GetFileAsString(string path);
        XDocument LoadXml(string path);

    }
}

