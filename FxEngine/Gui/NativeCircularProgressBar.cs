using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace FxEngine.Gui
{
    public class NativeCircularProgressBar : NativeGlGuiElement
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
            GL.Begin(PrimitiveType.TriangleFan);
            var cx = x + Rect.Width / 2;
            var cy = y + Rect.Height / 2;
            Drawer.Vertex(cx, cy);
            var rad = Math.Min(Rect.Width, Rect.Height);
            for (float i = 0; i <= Percentage; i += 0.02f)
            {
                var ang = i * 360f-90;
                var xx = (float)(cx + rad * Math.Cos(ang * Math.PI / 180.0f));
                var yy = (float)(cy + rad * Math.Sin(ang * Math.PI / 180.0f));
                Drawer.Vertex(xx,yy);
            }            
            
            GL.End();

            GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);

            GL.Begin(PrimitiveType.LineLoop);
            
            
            
            for (float i = 0; i <= 1.0f; i += 0.02f)
            {
                var ang = i * 360f-90;
                var xx = (float)(cx + rad * Math.Cos(ang * Math.PI / 180.0f));
                var yy = (float)(cy + rad * Math.Sin(ang * Math.PI / 180.0f));
                Drawer.Vertex(xx, yy);
            }

            GL.End();

            GL.PopMatrix();
        }

        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }

}
