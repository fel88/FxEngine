using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FxEngine.Shaders
{
    public class Shader : IShader
    {
        public static string ReadResourceTxt(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fr1 = assembly.GetManifestResourceNames().First(z => z.Contains(resourceName));

            using (Stream stream = assembly.GetManifestResourceStream(fr1))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public Shader InitFromShaderCodes(string vShaderCode, string fShaderCode)
        {
            // 2. compile shaders
            int vertex, fragment;
            // vertex shader

            vertex = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(vertex, vShaderCode);
            GL.CompileShader(vertex);
            checkCompileErrors(vertex, "VERTEX");
            // fragment Shader
            fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fShaderCode);
            GL.CompileShader(fragment);
            checkCompileErrors(fragment, "FRAGMENT");
            // shader Program
            ID = GL.CreateProgram();
            GL.AttachShader(ID, vertex);
            GL.AttachShader(ID, fragment);
            GL.LinkProgram(ID);
            checkCompileErrors(ID, "PROGRAM");
            // delete the shaders as they're linked into our program now and no longer necessary
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
            return this;
        }
        int ID;
        public void use()
        {
            GL.UseProgram(ID);
        }
        public void setMat4(string v, Matrix4 projection)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ID, v), false, ref projection);
        }
        public void setVec3(string v, Vector3 newPos)
        {
            GL.Uniform3(GL.GetUniformLocation(ID, v), ref newPos);
        }
        void checkCompileErrors(int shader, string type)
        {
            int success;
            string infoLog;
            if (type != "PROGRAM")
            {
                GL.GetShader(shader, ShaderParameter.CompileStatus, out success);
                if (success == 0)
                {
                    GL.GetShaderInfoLog(shader, out infoLog);
                    //std::cout << "ERROR::SHADER_COMPILATION_ERROR of type: " << type << "\n" << infoLog << "\n -- --------------------------------------------------- -- " << std::endl;
                }
            }
            else
            {
                GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out success);
                if (success == 0)
                {
                    GL.GetProgramInfoLog(shader, out infoLog);
                    throw new Exception(infoLog);
                    //  std::cout << "ERROR::PROGRAM_LINKING_ERROR of type: " << type << "\n" << infoLog << "\n -- --------------------------------------------------- -- " << std::endl;
                }
            }
        }
        public Shader InitFromResources(string vShaderResourceName, string fShaderResourceName)
        {
            var vShader = ReadResourceTxt(vShaderResourceName);
            var fShader = ReadResourceTxt(fShaderResourceName);
            InitFromShaderCodes(vShader, fShader);
            return this;
        }
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
