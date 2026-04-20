using OpenTK.Graphics.OpenGL;

namespace FxEngine.Loaders.Collada
{
    public class TextureLocal
    {

        public int textureId;
        public int size;
        private TextureTarget type;

        protected TextureLocal(int textureId, int size)
        {
            this.textureId = textureId;
            this.size = size;
            this.type = TextureTarget.Texture2D;
        }

        protected TextureLocal(int textureId, TextureTarget type, int size)
        {
            this.textureId = textureId;
            this.size = size;
            this.type = type;
        }

        public void bindToUnit(int unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(type, textureId);
        }

        public void delete()
        {
            GL.DeleteTexture(textureId);
        }

        

    }
}


