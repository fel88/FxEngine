using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace FxEngine.Gui
{
    public class NativeProgressBar : NativeGlGuiElement
    {
        BaseGlDrawingContext DrawingContext;

        public float Percentage;

        public override void Draw(BaseGlDrawingContext dc)
        {
            if (!Visible) return;
            DrawingContext = dc;
            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();

            if (Percentage > 1)
            {
                Percentage = 1;
            }
            if (Percentage < 0)
            {
                Percentage = 0;
            }

            var x = Rect.Left;

            var top = Rect.Top;

            var y = top;
            var r = Rect.Right;
            var d = Rect.Bottom;
            GL.PushMatrix();
            GL.Color3(Color.Beige);
            GL.Begin(PrimitiveType.Quads);
            Drawer.Vertex(x, d);
            Drawer.Vertex(x + Rect.Width * Percentage, d);
            Drawer.Vertex(x + Rect.Width * Percentage, y);
            Drawer.Vertex(x, y);
            GL.End();

            GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);

            Drawer.DrawRectangle(Rect);

            GL.PopMatrix();
        }

        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }

}
