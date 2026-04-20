using OpenTK.Graphics.OpenGL;

namespace FxEngine.Loaders.Collada
{    
    public class Vbo
    {

        private int vboId;
        private BufferTarget type;

        private Vbo(int vboId, BufferTarget type)
        {
            this.vboId = vboId;
            this.type = type;
        }

        public static Vbo create(BufferTarget type)
        {
            int[] ids = new int[1];
            GL.GenBuffers(1, ids);
            int id = ids[0];
            return new Vbo(id, type);
        }

        public void bind()
        {
            GL.BindBuffer(type, vboId);
        }

        public void unbind()
        {
            GL.BindBuffer(type, 0);
        }

        public void _storeData(float[] data)
        {
            //FloatBuffer buffer = BufferUtils.createFloatBuffer(data.length);
            //buffer.put(data);
            //buffer.flip();
            //storeData(buffer);
        }

        public void _storeData(int[] data)
        {
            //  IntBuffer buffer = BufferUtils.createIntBuffer(data.length);
            // buffer.put(data);
            //  buffer.flip();
            //  storeData(buffer);
        }

        public void storeData(int[] data)
        {
            GL.BufferData(type, data.Length, data, BufferUsageHint.StaticDraw);
        }

        public void storeData(float[] data)
        {
            GL.BufferData(type, data.Length, data, BufferUsageHint.StaticDraw);
        }

        public void delete()
        {
            GL.DeleteBuffer(vboId);
        }

    }
}


