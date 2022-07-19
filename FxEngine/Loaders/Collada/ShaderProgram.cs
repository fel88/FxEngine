using OpenTK.Graphics.OpenGL;
using System;

namespace FxEngine.Loaders.Collada
{
    public class ShaderProgram
    {

        private int programID;

        public ShaderProgram(MyFile vertexFile, MyFile fragmentFile, string[] inVariables)
        {
            int vertexShaderID = loadShader(vertexFile, ShaderType.VertexShader);
            int fragmentShaderID = loadShader(fragmentFile, ShaderType.FragmentShader);
            programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);
            bindAttributes(inVariables);
            GL.LinkProgram(programID);
            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
        }

        public void storeAllUniformLocations(Uniform[] uniforms)
        {
            foreach (Uniform uniform in uniforms)
            {
                uniform.storeUniformLocation(programID);
            }
            GL.ValidateProgram(programID);
        }

        public void start()
        {
            GL.UseProgram(programID);
        }

        public void stop()
        {
            GL.UseProgram(0);
        }

        public void cleanUp()
        {
            stop();
            GL.DeleteProgram(programID);
        }

        private void bindAttributes(String[] inVariables)
        {
            for (int i = 0; i < inVariables.Length; i++)
            {
                GL.BindAttribLocation(programID, i, inVariables[i]);
            }
        }

        private int loadShader(MyFile file, ShaderType type)
        {

            //get resource here
            var txt = ResourceFile.GetFileText(file.name);
            //var txt = File.ReadAllText(file.ToString());
            int shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, txt);
            GL.CompileShader(shaderID);

            //if (GL.GetShader(shaderID, ShaderParameter.CompileStatus) == GL11.GL_FALSE)
            int[] _params = new int[1];
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, _params);
            if (_params[0] != 1)
            {
                //System.out.println(GL20.glGetShaderInfoLog(shaderID, 500));
                string info;
                GL.GetShaderInfoLog(shaderID, out info);
                //System.err.println("Could not compile shader " + file);
                throw new Exception();
                //System.exit(-1);
            }
            return shaderID;
        }


    }
}


