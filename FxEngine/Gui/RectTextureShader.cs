using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using FxEngine.Shaders;

namespace FxEngine.Gui
{
    public class RectTextureShader : Shader
    {
        public override void Init()
        {
            base.Init("rectTexture.vs", "rectTexture.fs");


        }


        public void SetTexture(int text)
        {
            var loc = GL.GetUniformLocation(shaderProgram, "diffuseMap");
            GL.Uniform1(loc, text);
        }

        Vector3 Color;
        public override void SetUniformsData()
        {
            SetVec3("color", Color);
        }

        public void SetTransform(Matrix4 mtr)
        {
            SetMatrix4("transform", mtr);
        }

        public void SetColor(Color color)
        {
            Color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
            SetUniformsData();
        }
    }
}
