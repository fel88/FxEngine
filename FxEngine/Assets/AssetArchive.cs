using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FxEngine.Assets
{
    public class AssetArchive : IDataProvider
    {
        public List<AssetFile> Files = new List<AssetFile>();
        public byte[] GetFile(string path)
        {
            var full = new FileInfo(path).FullName.ToLower();
            var ar1 = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var fr = Files.First(z => z.Path.ToLower() == full);
            

            return fr.Data;
        }

        public void AppendFile(AssetFile f)
        {
            Files.Add(f);
        }
        public void AppendFile(string path)
        {
            if (Files.Any(z => z.Path == path)) return;
            var bts = File.ReadAllBytes(path);
            var fs = new FileInfo(path);
            AppendFile(new AssetFile(fs.FullName, bts));
        }
        public void LoadFromFile(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                string str = "FXENGINE.ASSET";
                var u = new UTF8Encoding();
                var bts = u.GetBytes(str);
                fs.Read(bts, 0, bts.Length);
                var len = ReadInt(fs);
                List<int> shifts = new List<int>();
                for (int i = 0; i < len; i++)
                {
                    var shift = ReadInt(fs);
                    shifts.Add(shift);
                }
                foreach (var item in shifts)
                {
                    fs.Seek(item, SeekOrigin.Begin);
                    Files.Add(ReadFile(fs));
                }
            }
        }

        AssetFile ReadFile(FileStream fs)
        {
            AssetFile ret = new AssetFile();
            var pathlen = ReadInt(fs);
            byte[] bb = new byte[pathlen];
            fs.Read(bb, 0, bb.Length);
            var enc = new UTF8Encoding();
            var str = enc.GetString(bb);

            var ar1 = str.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            ret.Path = str;
            ret.Name = ar1.Last();
            var len = ReadInt(fs);
            byte[] bb2 = new byte[len];
            fs.Read(bb2, 0, bb2.Length);
            ret.Data = bb2;

            return ret;
        }
        int ReadInt(FileStream fs)
        {
            byte[] bb = new byte[4];
            fs.Read(bb, 0, 4);
            return BitConverter.ToInt32(bb, 0);
        }

        public void SaveToFile(string v)
        {
            using (FileStream fs = new FileStream(v, FileMode.Create))
            {
                string str = "FXENGINE.ASSET";
                var u = new UTF8Encoding();
                var bts = u.GetBytes(str);
                fs.Write(bts, 0, bts.Length);
                //count of files
                var cnt = BitConverter.GetBytes(Files.Count);
                fs.Write(cnt, 0, cnt.Length);
                int pos = (int)fs.Position + Files.Count * 4;
                //write table
                foreach (var item in Files)
                {
                    var hd = item.GetHeader();

                    var bts2 = BitConverter.GetBytes(pos);
                    fs.Write(bts2, 0, bts2.Length);
                    pos += hd.Length;
                    pos += item.Data.Length;
                }


                foreach (var item in Files)
                {
                    var hd = item.GetHeader();
                    fs.Write(hd, 0, hd.Length);
                    fs.Write(item.Data, 0, item.Data.Length);
                }
            }
        }

        public XDocument LoadXml(string path)
        {            
            return XDocument.Parse(GetFileAsString(path));
        }

        public Bitmap GetBitmap(string path)
        {
            var enc = new UTF8Encoding();
            var p = GetFile(path);
            var mems = new MemoryStream(p);
            return Bitmap.FromStream(mems) as Bitmap;

        }

        public string GetFileAsString(string path)
        {
            var enc = new UTF8Encoding();
            var p = GetFile(path);
            var str = enc.GetString(p);
            return str;
        }
    }
}
