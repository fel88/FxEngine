using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace FxEngine.Loaders.Collada
{
    public class UniformMatrix : Uniform
    {


        ////ivate static FloatBuffer matrixBuffer = BufferUtils.createFloatBuffer(16);

        public UniformMatrix(String name) : base(name)
        {

        }

        public void loadMatrix(Matrix4 matrix)
        {
            //matrix.store(matrixBuffer);
            //matrixBuffer.flip();
            //matrix.Invert();??
            GL.UniformMatrix4(getLocation(), false, ref matrix);
        }



    }
}


