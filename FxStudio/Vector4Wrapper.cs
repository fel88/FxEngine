using OpenTK.Mathematics;
namespace FxEngineEditor
{
    public class Vector4Wrapper
    {
        public Vector4 Vector = new Vector4();
        public float X { get { return Vector.X; } set { Vector.X = value; } }
        public float Y { get { return Vector.Y; } set { Vector.Y = value; } }
        public float Z { get { return Vector.Z; } set { Vector.Z = value; } }
        public float W { get { return Vector.W; } set { Vector.W = value; } }
    }


}
