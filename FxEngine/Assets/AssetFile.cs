using System;
using System.Collections.Generic;
using System.Text;

namespace FxEngine.Assets
{
    public class AssetFile
    {        
        public byte[] GetHeader()
        {
            List<byte> bb = new List<byte>();
            var enc = new UTF8Encoding();
            var p = enc.GetBytes(Path);
            bb.AddRange(BitConverter.GetBytes(p.Length));
            bb.AddRange(p);
            bb.AddRange(BitConverter.GetBytes(Data.Length));
            return bb.ToArray();
        }

        public AssetFile()
        {

        }

        public AssetFile(string path, byte[] data)
        {
            Path = path;
            Data = data;
        }
        public string Name;
        public string Path;
        public byte[] Data;
    }
}
