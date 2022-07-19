namespace FxEngine.Loaders.Collada
{
    public class ColladaNameAccessor : IColladaAccessor
    {
        public ColladaSource Source;
        public ColladaNameAccessor(ColladaSource ar)
        {
            Source = ar;
        }

        public dynamic GetElement(int index)
        {
            return (Source.Array as ColladaNameArray).Names[Source.Stride * index];
        }
    }
}


