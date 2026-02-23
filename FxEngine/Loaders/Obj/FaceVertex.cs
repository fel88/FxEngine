using OpenTK;
using OpenTK.Mathematics;

namespace FxEngine.Loaders.OBJ
{
    public class FaceVertex
    {
        public Vector3d Position;
        public Vector3d Normal;
        public Vector2d TextureCoord;

        public TempVertex Temp;
        public FaceVertex()
        {

        }

        public FaceVertex(Vector3d pos, Vector3d norm, Vector2d texcoord, TempVertex temp = null)
        {
            Position = pos;
            Normal = norm;
            TextureCoord = texcoord;
            Temp = temp;
        }
    }
}