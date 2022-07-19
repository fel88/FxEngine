using OpenTK;
using OpenTK.Graphics.OpenGL;
using FxEngine.Cameras;

namespace FxEngine.Loaders.Collada
{
    public class SkyboxRenderer
    {
        private SkyboxShader shader;

        public SkyboxRenderer()
        {
            this.shader = new SkyboxShader();
        }

        /**
         * Renders the skybox.
         * 
         * @param camera
         *            - the scene's camera.
         */
        public void render(Vao box, Camera camera)
        {
            prepare(camera);
            box.bind(new int[] { 0 });
            GL.DrawElements(PrimitiveType.Triangles, box.getIndexCount(), DrawElementsType.UnsignedInt, 0);
            box.unbind(new int[] { 0 });
            shader.stop();
        }

        /**
         * Delete the shader when the game closes.
         */
        public void cleanUp()
        {
            shader.cleanUp();
        }

        /**
         * Starts the shader, loads the projection-view matrix to the uniform
         * variable, and sets some OpenGL state which should be mostly
         * self-explanatory.
         * 
         * @param camera
         *            - the scene's camera.
         */
        private void prepare(Camera camera)
        {
            shader.start();
            Matrix4 matrix;

            GL.GetFloat(GetPName.ModelviewMatrix, out matrix);
            Matrix4 m = matrix * camera.ProjectionMatrix;

            shader.projectionViewMatrix.loadMatrix(m);

            //OpenGlUtils.disableBlending();
            //OpenGlUtils.enableDepthTesting(true);
            //OpenGlUtils.cullBackFaces(true);
            //OpenGlUtils.antialias(false);
        }

    }
}


