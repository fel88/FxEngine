using System.Drawing;
using System.IO.Compression;
using System.IO;
using System.Xml.Linq;
using System;
using System.Linq;

namespace FxEngine
{
    public class ZipAssetFilesystemDataProvider : IDataProvider, IDisposable
    {
        ZipArchive Archive;
        public ZipAssetFilesystemDataProvider(string path)
        {
            Archive = ZipFile.Open(path, ZipArchiveMode.Read);
        }
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                Archive.Dispose();
            }

            disposed = true;
        }

        ~ZipAssetFilesystemDataProvider()
        {
            Dispose(false);
        }

        public Bitmap GetBitmap(string path)
        {
            var b = GetFile(path);
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(b, 0, b.Length);
                return Bitmap.FromStream(ms) as Bitmap;
            }
        }

        public byte[] GetFile(string path)
        {
            var ent = Archive.GetEntry(path);
            using (var stream = ent.Open())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public string GetFileAsString(string path)
        {
            var ent = Archive.GetEntry(path);
            using (var stream = ent.Open())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public XDocument LoadXml(string path)
        {
            return XDocument.Parse(GetFileAsString(path));
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public bool IsFileExists(string amb1)
        {
            return Archive.Entries.Any(z => z.Name == amb1);
        }
    }
}

