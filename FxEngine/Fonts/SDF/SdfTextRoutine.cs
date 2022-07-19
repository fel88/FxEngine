using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace FxEngine.Fonts.SDF
{
    public class SdfTextRoutine : TextRenderer
    {
        public SdfTextRoutine()
        {
            shader = new SdfShader();
        }
        
        public override void RenderChar(char c, PointF pos)
        {
            if (!infos.ContainsKey(c)) return;
            var fr = infos[c];
            
            GL.PushMatrix();
            GL.Disable(EnableCap.DepthTest);
            
            var offset = fr.Char.Offset;

            GL.Translate(0, -fr.Size.Height, 0);
            GL.Translate(pos.X, pos.Y, 0);
            GL.Translate(offset.X, -offset.Y, 0);
                        
            (shader as SdfShader).SetUniformsData();
            GL.BindVertexArray(fr.VertexBuffer);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.PopMatrix();
        }

        public override void Init(Graphics gr)
        {
            atlas.Load("atlas.png", "atlas.xml");
            Font = atlas.Font;
            kr = new KerningRoutine(gr, Font);
            shader.Init();
            InitCharTextures();
        }

        public SdfShader Shader
        {
            get
            {
                return shader as SdfShader;
            }
        }

        public void SetGamma(float v)
        {            
            Shader._gamma = v;
        }
    }
}
