using OpenTK;
using System.Text;

namespace FxEngine
{
    public static class Extensions
    {
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
