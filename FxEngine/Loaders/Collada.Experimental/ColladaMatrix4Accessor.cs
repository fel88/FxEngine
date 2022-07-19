using OpenTK;
using System.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaMatrix4Accessor : IColladaAccessor
    {
        public ColladaSource Source;
        public ColladaMatrix4Accessor(ColladaSource ar)
        {
            Source = ar;
        }

        public dynamic GetElement(int index)
        {
            var arr = Source.AsFloatArray.Array.Skip(index * Source.Stride).Take(Source.Stride).ToArray();
            return new Matrix4(
                    new Vector4(arr[0], arr[1], arr[2], arr[3]),
                    new Vector4(arr[4], arr[5], arr[6], arr[7]),
                    new Vector4(arr[8], arr[9], arr[10], arr[11]),
                    new Vector4(arr[12], arr[13], arr[14], arr[15])
                    );
        }
    }
}


