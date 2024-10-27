using System.Linq;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Mathematics;

namespace FxEngine.Gui
{
    public class NativeButton : AbstractNativeButton
    {
        public override void Draw(BaseGlDrawingContext dc)
        {
            if (!Visible) return;
            DrawingContext = dc;
            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();

            GL.PushMatrix();
            if (!Enabled)
            {
                GL.Color3(Color.LightGray);
            }
            else
            {
                if (IsHovered)
                {
                    GL.Color3(Color.FromArgb(73, 83, 89));
                }
                else
                {
                    GL.Color3(Color.FromArgb(53, 63, 69));
                }
            }
            Drawer.FillRectangle(Rect);

            GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);
            Drawer.DrawRectangle(Rect);

            GL.LineWidth(1);
            GL.PushMatrix();

            float f1 = TextScale;
            Drawer.Translate(Rect.Left, Rect.Top);
            var rgs = dc.TextRoutine.kr.GetStringRegions(Caption);
            var width = rgs.Last().Bound.Right * f1;
            if (TextAlign == GlGuiTextAlign.Center)
            {
                GL.Translate(Rect.Width / 2 - width / 2, 0, 0);
            }
            GL.Scale(f1, f1, f1);


            dc.TextRoutine.Shader.PushState();
            dc.TextRoutine.Shader.Color = new Vector3(1, 1, 1);
            dc.TextRoutine.DrawText(Caption, new PointF(0, 0));
            dc.TextRoutine.Shader.PopState();
            GL.PopMatrix();
            GL.PopMatrix();
        }

        public float TextScale = 0.3f;
     
    }
}
