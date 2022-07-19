using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace FxEngine.Gui
{
    public class NativeTrackBar : NativeGlGuiElement
    {
        public override void Event(BaseGlDrawingContext dc, GlGuiEvent ev)
        {
            if (!Visible) return;
            IsHovered = false;
            if (Rect.IntersectsWith(ev.Position.X,  ev.Position.Y))
            {
                IsHovered = true;
            }

            if (ev is MouseClickGlGuiEvent)
            {
                if (IsHovered)
                {
                    Value = (ev.Position.X - Rect.Left) / Rect.Width;
                    if (ValueChanged != null)
                    {
                        ValueChanged(this);
                    }
                    ev.Handled = true;
                }
            }
            base.Event(dc, ev);
        }
        public bool IsHovered;
        BaseGlDrawingContext DrawingContext;
        

        public Action<NativeTrackBar> ValueChanged;

        public float Value;

        public override void Draw(BaseGlDrawingContext dc)
        {
            DrawingContext = dc;
            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();
            

            if (Value > 1)
            {
                Value = 1;
            }
            if (Value < 0)
            {
                Value = 0;
            }

            var x = Rect.Left;

            var top = Rect.Top;

            var y =  top;
            var r = Rect.Right;
            var d = Rect.Bottom;
            GL.PushMatrix();
            GL.Color3(Color.Beige);
            GL.Begin(PrimitiveType.Quads);
            Drawer.Vertex(x, d);
            Drawer.Vertex(x+Rect.Width*Value, d);
            Drawer.Vertex(x+Rect.Width*Value, y);
            Drawer.Vertex(x, y);
            
            GL.End();



            GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);
            Drawer.DrawRectangle(Rect);
            /*GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(x, d, 0);
            GL.Vertex3(r, d, 0);
            GL.Vertex3(r, y, 0);
            GL.Vertex3(x, y, 0);
            GL.End();*/
            int ww = 10;

            GL.Color3(Color.FromArgb(118, 141, 155));
            if (IsHovered)
            {
                GL.Color3(Color.FromArgb(158, 181, 195));
            }
            GL.Begin(PrimitiveType.Quads);

            Drawer.Vertex(x + Rect.Width * Value - ww / 2, d + ww / 2);
            Drawer.Vertex(x + Rect.Width * Value + ww / 2, d + ww / 2);
            Drawer.Vertex(x + Rect.Width * Value + ww / 2, y - ww / 2);
            Drawer.Vertex(x + Rect.Width * Value - ww / 2, y - ww / 2);
            GL.End();
            GL.PopMatrix();
        }

        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }
}
