using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace FxEngine.Gui
{
    public class NativeCheckBox : NativeGlGuiElement
    {
        public override void Event(BaseGlDrawingContext dc, GlGuiEvent ev)
        {
            if(Parent is NativePanel)
            {
                if(!(Parent as NativePanel).IsChildsVisible)
                {
                    return;
                }
            }
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
                    IsChecked = !IsChecked;
                    if (CheckedChanged != null)
                    {

                        CheckedChanged(this);
                    }
                    ev.Handled = true;
                }
            }
            base.Event(dc, ev);
        }
        public bool IsChecked;
        public bool IsHovered = false;
        public string Caption;

        BaseGlDrawingContext DrawingContext;
        

        public Action<NativeCheckBox> CheckedChanged { get; set; }

        public override void Draw(BaseGlDrawingContext dc)
        {
            if (!Visible) return;
            DrawingContext = dc;
            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();


            var x = Rect.Left;
            var top = Rect.Top;

            
            var r = Rect.Right;
            var d =  Rect.Bottom;
            GL.PushMatrix();


            if (IsHovered)
            {
                GL.Color3(Color.FromArgb(73, 83, 89));
            }
            else
            {
                GL.Color3(Color.FromArgb(53, 63, 69));
            }
            Drawer.FillRectangle(Rect);
            /*
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex3(x, d, 0);
            GL.Vertex3(r, d, 0);
            GL.Vertex3(r, y, 0);
            GL.Vertex3(x, y, 0);
            GL.End();*/
            GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);
            Drawer.DrawRectangle(Rect);
            /*GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(x, d, 0);
            GL.Vertex3(r, d, 0);
            GL.Vertex3(r, y, 0);
            GL.Vertex3(x, y, 0);
            GL.End();
            */
            if (IsChecked)
            {
                
                GL.Begin(PrimitiveType.Lines);
                Drawer.Vertex(x, d);
                Drawer.Vertex(r, top);
                Drawer.Vertex(x, top);
                Drawer.Vertex(r, d);                
                GL.End();               
            }
            GL.LineWidth(1);
            GL.PushMatrix();
            float f1 = 0.3f;

            Drawer.Translate(x + 30, Rect.Top);
            //GL.Translate(x + 30, dc.GameWindow.Height - (Rect.Top), 0);
            GL.Scale(f1, f1, f1);

            dc.TextRoutine.Shader.PushState();
            dc.TextRoutine.Shader.Color = new Vector3(1, 1, 1);
            dc.TextRoutine.DrawText(Caption, new PointF(0, 0));
            dc.TextRoutine.Shader.PopState();
            GL.PopMatrix();
            GL.PopMatrix();
        }

        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }
}
