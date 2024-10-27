using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.Drawing;
using System.Text;

namespace FxEngine
{
    public static class Extensions
    {
        public static Rectangle ToRectangle(this Box2i box)
        {
            return new Rectangle(0, 0, box.Size.X, box.Size.Y);
        }
        public static Vector2i ToVector2i(this Point p)
        {
            return new Vector2i(p.X, p.Y);
        }
        public static Point ToPoint(this Vector2i p)
        {
            return new Point(p.X, p.Y);
        }
        public static int Width(this GameWindow box)
        {
            return box.Size.X;
        }
        public static int Height(this GameWindow box)
        {
            return box.Size.Y;
        }
        public static string ToXml(this Matrix4 mat)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                sb.AppendLine(mat.Row0[i] + ";" + mat.Row1[i] + ";" + mat.Row2[i] + ";" + mat.Row3[i]);
            }
            return sb.ToString();
        }
    }
}
