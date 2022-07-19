using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FxEngine.Shaders
{
    public class ModelDrawShader : Shader
    {
        string Vs;
        string Fs;
        public ModelDrawShader(string vs, string fs)
        {
            Fs = fs;
            Vs = vs;
        }
        public static int ShaderProgram;
        public int shaderProg
        {
            get
            {
                return shaderProgram;
            }
        }




        public override void Init()
        {
            Init(Vs, Fs);
            ShaderProgram = shaderProgram;
        }

        public override void SetUniformsData()
        {

            int unif_mult;

            var unif_name = "mult";
            unif_mult = GL.GetUniformLocation(shaderProgram, unif_name);
            if (unif_mult == -1)
            {

            }

            GL.Uniform1(unif_mult, ColorMultipler);

            unif_name = "opacity";
            unif_mult = GL.GetUniformLocation(shaderProgram, unif_name);
            if (unif_mult == -1)
            {

            }

            GL.Uniform1(unif_mult, Opacity);
            SetMatrix4("model", Model);
        }
        public Matrix4 Model;
        public float ColorMultipler = 1;
        public float Opacity = 1.0f;


    }

 
    
}
