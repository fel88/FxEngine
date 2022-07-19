using System.Linq;

namespace FxEngine.Loaders.Collada
{
    public class ColladaFloatAccessor : IColladaAccessor
    {
        public ColladaSource Source;
        public ColladaFloatAccessor(ColladaSource ar)
        {
            Source = ar;
        }

        public dynamic GetElement(int index)
        {
            return Source.AsFloatArray.Array.Skip(index * Source.Stride).Take(Source.Stride).ToArray();
        }
    }
}


