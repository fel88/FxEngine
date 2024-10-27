using OpenTK.Mathematics;

namespace FxEngine
{
    public static class OpentkExtensions
    {
        public static Vector3 ToVector3(this Vector3d v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }
        public static Vector3d ToVector3d(this Vector3 v)
        {
            return new Vector3d(v.X, v.Y, v.Z);
        }

        public static Vector2d ToVector2d(this Vector2 v)
        {
            return new Vector2d(v.X, v.Y);
        }

    }
}
