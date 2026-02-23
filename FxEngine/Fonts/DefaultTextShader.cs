using FxEngine.Shaders;

namespace FxEngine.Fonts
{
    public class DefaultTextShader : Shader
    {
        public DefaultTextShader()
        {
            InitFromResources("text.vs", "text.fs");
        }
    }
}
