using System.Drawing;
using System.Xml.Linq;

namespace FxEngine
{
    public interface IDataProvider
    {
        string GetDirectoryName(string path);
        Bitmap GetBitmap(string path);
        byte[] GetFile(string path);
        string GetFileAsString(string path);
        XDocument LoadXml(string path);
        bool IsFileExists(string amb1);
    }
}

