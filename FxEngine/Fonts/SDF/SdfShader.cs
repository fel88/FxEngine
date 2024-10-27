using FxEngine.Shaders;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FxEngine.Fonts.SDF
{
    public class SdfShader : Shader, ITextShader
    {
        public float _gamma = 0.005f;

        public override void SetUniformsData()
        {
            Matrix4 m;
            GL.GetFloat(GetPName.ModelviewMatrix, out m);
            Matrix4 m2;
            GL.GetFloat(GetPName.ProjectionMatrix, out m2);

            GL.UniformMatrix4(Unif_proj, false, ref m2);
            GL.UniformMatrix4(Unif_trans, false, ref m);            
        }

        public Vector3 Color;

        public struct LocalShaderInfo
        {
            public Vector3 Color;
            public float Gamma;
        }


        public Stack<LocalShaderInfo> Stack = new Stack<LocalShaderInfo>();
        public void PushState()
        {
            Stack.Push(new LocalShaderInfo() { Color = Color, Gamma = _gamma });
        }
        public void PopState()
        {
            var pop = Stack.Pop();
            Color = pop.Color;
            _gamma = pop.Gamma;
        }

        public override void Init()
        {
            var vesr = GL.GetString(StringName.Version);
            
            string vsSource = @"attribute vec2 coord; void main() { gl_Position = vec4(coord, 0.0, 1.0); };";
            string fsSource = @"uniform vec4 color; void main() { gl_FragColor = color; }";                        

            var asm = Assembly.GetAssembly(typeof(SdfShader));
                        
            var resourceName = "FxEngine.Graphics.Shaders.sdf.vs";
            using (Stream stream = asm.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                vsSource = reader.ReadToEnd();
            }
            resourceName = "FxEngine.Graphics.Shaders.sdf.fs";
            using (Stream stream = asm.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                fsSource = reader.ReadToEnd();
            }
                        
            int vShader, fShader; 
            vShader = GL.CreateShader(ShaderType.VertexShader);
            
            GL.ShaderSource(vShader, vsSource);
            { var er = GL.GetError(); }
            
            GL.CompileShader(vShader);
            { var er = GL.GetError(); }
            ShaderLog("vertex shader", vShader);
            
            fShader = GL.CreateShader(ShaderType.FragmentShader);
            
            GL.ShaderSource(fShader, fsSource);
            { var er = GL.GetError(); }
            
            GL.CompileShader(fShader);
            { var er = GL.GetError(); }
            ShaderLog("fragment shader", fShader);
            
            shaderProgram = GL.CreateProgram();

            GL.AttachShader(shaderProgram, vShader);
            { var er = GL.GetError(); }
            GL.AttachShader(shaderProgram, fShader);
            { var er = GL.GetError(); }

            
            GL.LinkProgram(shaderProgram);

            { var er = GL.GetError(); }
            int link_ok;
            GL.GetShader(fShader, ShaderParameter.CompileStatus, out link_ok);

            if (link_ok == 0)
            {
                throw new Exception("fshader compile error");
                return;
            }
            GL.GetShader(vShader, ShaderParameter.CompileStatus, out link_ok);
            if (link_ok == 0)
            {
                throw new Exception("vshader compile error");
                return;
            }
            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out link_ok);

            if (link_ok == 0)
            {
                throw new Exception("shader link error");
                return;
            }

            
            string attr_name = "texSDFCoords";


            Attrib_texSDFCoords = GL.GetAttribLocation(shaderProgram, attr_name);
            if (Attrib_texSDFCoords == -1)
            {
                //throw new Exception(string.Format("could not bind attrib {0}", attr_name));
                //  return;
            }

            attr_name = "position";

            Attrib_position = GL.GetAttribLocation(shaderProgram, attr_name);
            if (Attrib_position == -1)
            {
                //throw new Exception(string.Format("could not bind attrib {0}", attr_name));
                // return;
            }
            //! Вытягиваем ID юниформ
            string unif_name = "texSDF";
            Unif_texSDF = GL.GetUniformLocation(shaderProgram, unif_name);
            if (Unif_texSDF == -1)
            {
                //  throw new Exception(string.Format("could not bind attrib {0}", unif_name));
            }
            unif_name = "gammaSDF";
            Unif_gammaSDF = GL.GetUniformLocation(shaderProgram, unif_name);
            if (Unif_gammaSDF == -1)
            {
                //  throw new Exception(string.Format("could not bind attrib {0}", unif_name));
            }

            unif_name = "color";
            Unif_color = GL.GetUniformLocation(shaderProgram, unif_name);
            if (Unif_color == -1)
            {
                //  throw new Exception(string.Format("could not bind attrib {0}", unif_name));
            }

            unif_name = "projection";
            Unif_proj = GL.GetUniformLocation(shaderProgram, unif_name);
            if (Unif_proj == -1)
            {
                //  throw new Exception(string.Format("could not bind attrib {0}", unif_name));
            }
            unif_name = "transformation";
            Unif_trans = GL.GetUniformLocation(shaderProgram, unif_name);
            if (Unif_trans == -1)
            {
                // throw new Exception(string.Format("could not bind attrib {0}", unif_name));
            }



            CheckOpenGLerror();
        }

        
        int Attrib_texSDFCoords; 
        int Attrib_position; 
        int Unif_texSDF; 
        int Unif_gammaSDF; //! ID        Vertex Buffer Object

        int Unif_proj; //! ID        Vertex Buffer Object
        int Unif_trans; //! ID        Vertex Buffer Object
        int Attrib_vertex;
        int Unif_color;

        public int AtlasTextureId { get; set; } = -1;

        
        void ShaderLog(string tag, int shader)
        {
            string infoLog; GL.GetShaderInfoLog(shader, out infoLog);
            if (infoLog.Length > 1)
            {
                // logs.Add(tag + " InfoLog: " + infoLog + "");
                //richTextBox1.AppendText(logs.Last());
            }
        }
        
        static void CheckOpenGLerror()
        {
            ErrorCode errCode = GL.GetError();
            if (errCode != ErrorCode.NoError)
                Console.WriteLine("OpenGl error! - {0}", errCode);
        }

        
        public void SetTexture()
        {
            GL.Uniform1(Unif_gammaSDF, _gamma);
            GL.Uniform3(Unif_color, Color);
            GL.Uniform1(Unif_texSDF, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, AtlasTextureId); 
        }
    }
}
