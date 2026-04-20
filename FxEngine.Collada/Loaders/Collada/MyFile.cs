using System;

namespace FxEngine.Loaders.Collada
{
    public class MyFile
    {
        private static String FILE_SEPARATOR = "/";

        public string path;
        public string name;

        public MyFile(MyFile file, String subFile)
        {
            this.path = file.path + FILE_SEPARATOR + subFile;
            this.name = subFile;
        }
        public MyFile(string file, string subFile)
        {
            this.path = file + FILE_SEPARATOR + subFile;
            this.name = subFile;
        }
    }
}


