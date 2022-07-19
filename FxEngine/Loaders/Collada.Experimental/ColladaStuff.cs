using OpenTK;

namespace FxEngine.Loaders.Collada
{
    public class ColladaStuff
    {
        public static Matrix4 MatrixFromArray(float[] arr, bool rowForawrd = false)
        {
            if (rowForawrd)
            {
                return new Matrix4(
                                    new Vector4(arr[0], arr[4], arr[8], arr[12]),
                                    new Vector4(arr[1], arr[5], arr[9], arr[13]),
                                    new Vector4(arr[2], arr[6], arr[10], arr[14]),
                                    new Vector4(arr[3], arr[7], arr[11], arr[15])
                                    );
            }
            else
            {
                return new Matrix4(
                                    new Vector4(arr[0], arr[1], arr[2], arr[3]),
                                    new Vector4(arr[4], arr[5], arr[6], arr[7]),
                                    new Vector4(arr[8], arr[9], arr[10], arr[11]),
                                    new Vector4(arr[12], arr[13], arr[14], arr[15])
                                    );
            }
        }
    }
}


