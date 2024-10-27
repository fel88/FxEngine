using OpenTK.Mathematics;
using System.Drawing;
using FxEngine.Shaders;

namespace FxEngine.Gui
{
    public class RectShader : Shader
    {
        public override void Init()
        {
            base.Init("rect.vs", "rect.fs");
        }

        Vector4 Color;
        public override void SetUniformsData()
        {
            SetVec4("color", Color);
        }
        public void SetTransform(Matrix4 mtr)
        {
            SetMatrix4("transform", mtr);
        }
        public void SetColor(Color color)
        {
            Color = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1);
            SetUniformsData();
        }
        public void SetColor(Vector4 color)
        {
            Color = color;
            SetUniformsData();
        }
    }
}
