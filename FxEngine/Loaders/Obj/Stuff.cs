using System;
using System.Collections.Generic;
using OpenTK;

namespace FxEngine.Loaders.OBJ
{
    public class Stuff
    {
        public static OpenTK.Quaternion ToQuaternion(object f)
        {
            if (f is Quaternion)
            {
                var q = (Quaternion)f;
                return new OpenTK.Quaternion(q.X, q.Y, q.Z, q.W);
            }
            if (f is OpenTK.Quaternion)
            {
                return (OpenTK.Quaternion)f;
            }

            throw new ArgumentException("cant cast " + f.GetType().Name + " to opentk.quaternion");
        }

        public static Vector3 ToVector3(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public static List<string> LoadedObjs = new List<string>();


        public static List<ModelPathItem> ModelsPathes = new List<ModelPathItem>();

        public static List<TextureDescriptor> Textures { get; set; } = new List<TextureDescriptor>();
    }
}
