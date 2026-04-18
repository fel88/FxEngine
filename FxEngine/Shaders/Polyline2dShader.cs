using System.Reflection;

namespace FxEngine.Shaders
{
    public class Polyline2dShader : Shader
    {
        public Polyline2dShader()
        {
            InitFromResources("shader_2d.vs", "shader_2d.fs");
        }

    }
}
