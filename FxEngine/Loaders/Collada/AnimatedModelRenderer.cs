using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FxEngine.Loaders.Collada
{
    public class AnimatedModelRenderer
    {
        public void Render(ColladaModel entity, ICamera camera, Vector3 lightDir)
        {
            //prepare(camera, lightDir);
            //entity.getTexture().bindToUnit(0);
            entity.Vao.bind(new int[] { 0, 1, 2, 3, 4 });
            //shader.jointTransforms.loadMatrixArray(entity.getJointTransforms());
            GL.DrawElements(PrimitiveType.Triangles, entity.Vao.getIndexCount(), DrawElementsType.UnsignedInt, 0);
            entity.Vao.unbind(new int[] { 0, 1, 2, 3, 4 });
            finish();
        }
        private void finish()
        {
            shader.stop();
        }
        public void cleanUp()
        {
            shader.cleanUp();
        }
        public AnimatedModelRenderer()
        {
            this.shader = new SkyboxShader();
        }
        private ShaderProgram shader;


    }
}


