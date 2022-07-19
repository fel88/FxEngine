using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Reflection;

namespace FxEngine.Shaders
{
    public class Shader : IShader
    {

        public void SetVec3(string nm, Vector3 v)
        {
            var loc = GL.GetUniformLocation(shaderProgram, nm);
            GL.Uniform3(loc, v);
        }
        public void SetVec4(string nm, Vector4 v)
        {
            var loc = GL.GetUniformLocation(shaderProgram, nm);
            GL.Uniform4(loc, v);
        }

        public void SetMatrix4(string nm, Matrix4 v)
        {
            var loc = GL.GetUniformLocation(shaderProgram, nm);            
            GL.UniformMatrix4(loc, false, ref v);
        }

        public int GetProgramId()
        {
            return shaderProgram;
        }
        public int shaderProgram;

        public virtual void Init()
        {

        }

        public void Init(string nm1 = "vertexshader1", string nm2 = "fragmentshader1")
        {            
            var asm = Assembly.GetAssembly(typeof(ModelDrawShader));
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            var vertexShaderSource = ResourceFile.GetFileText(nm1, asm);
            var fragmentShaderSource = ResourceFile.GetFileText(nm2, asm);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            
            int success;
            string infoLog;

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                GL.GetShaderInfoLog(vertexShader, out infoLog);
                throw new Exception(infoLog);
                
            }
            
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                GL.GetShaderInfoLog(fragmentShader, out infoLog);
                throw new Exception(infoLog);                
            }
            
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            
            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out success);
            
            if (success == 0)
            {
                GL.GetProgramInfoLog(shaderProgram, out infoLog);
                throw new Exception(infoLog);                
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public virtual void SetUniformsData()
        {

        }

        public void Use()
        {
            GL.UseProgram(shaderProgram);
        }
    }
}
