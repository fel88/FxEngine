namespace FxEngine.Loaders.Collada
{
    public class SkyboxShader : ShaderProgram
    {
        private static MyFile VERTEX_SHADER = new MyFile("skybox", "skyboxVertex.glsl");
        private static MyFile FRAGMENT_SHADER = new MyFile("skybox", "skyboxFragment.glsl");

        public UniformMatrix projectionViewMatrix = new UniformMatrix("projectionViewMatrix");

        public SkyboxShader() : base(VERTEX_SHADER, FRAGMENT_SHADER, new string[] { "in_position" })
        {
            storeAllUniformLocations(new[] { projectionViewMatrix });
        }
    }
}


