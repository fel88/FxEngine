using OpenTK.Graphics.OpenGL;

namespace FxEngine
{
    public static class Helpers
    {
        public static bool IsCompatibilityMode()
        {
            int numext;
            GL.GetInteger(GetPName.NumExtensions, out numext);
            for (int i = 0; i < numext; i++)
            {
                var res1 = GL.GetString(StringNameIndexed.Extensions, i);
                if (res1 == "GL_ARB_compatibility")
                {
                    //forawrd
                    return true;
                }
            }
            return false;
        }
    }
}
