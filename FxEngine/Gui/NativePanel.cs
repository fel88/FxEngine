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
    public class NativePanel : NativeGlGuiElement
    {
        public override bool ContainFocus
        {
            get
            {
                return Childs.Any(z => z.Focused) || Childs.OfType<NativePanel>().Any(z => z.ContainFocus);
            }
        }

        public bool IsChildsVisible
        {
            get
            {
                if (Parent is NativePanel)
                {
                    return (Parent as NativePanel).IsChildsVisible && IsExpanded;
                }
                return IsExpanded;
            }
        }

        public override void ResetFocus()
        {
            foreach (var item in Childs)
            {
                item.ResetFocus();
            }
            base.ResetFocus();
        }
        public override void Event(BaseGlDrawingContext dc, GlGuiEvent ev)
        {
            if (!Visible) return;
            IsHovered = false;
            var g = new GuiBounds(Rect.XOffset, Rect.YOffset, Rect.Width, titleH, Rect.Anchor);
            g.Parent = Rect.Parent;
            g.Update();

            bool ishvrd2 = false;
            if (g.IntersectsWith(ev.Position.X, ev.Position.Y))
            {
                IsHovered = true;
            }

            if (Rect.IntersectsWith(ev.Position.X, ev.Position.Y))
            {
                ishvrd2 = true;
            }

            if (ev is MouseClickGlGuiEvent)
            {
                if (ishvrd2)
                {
                    ev.Handled = true;
                }
                if (IsHovered && IsExpandable)
                {

                    IsExpanded = !IsExpanded;
                    if (IsExpanded)
                    {
                        Rect.Height = TempHeight;
                    }
                    else
                    {
                        TempHeight = Rect.Height;
                        Rect.Height = titleH;
                    }

                    ev.Handled = true;
                }
            }
            foreach (var item in Childs)
            {
                item.Event(dc, ev);
            }
            base.Event(dc, ev);
        }
        public float TempHeight = 0;
        public string Title;
        public bool IsExpanded = true;

        public bool IsHovered = false;

        public int titleH = 25;
        BaseGlDrawingContext DrawingContext;

        public bool IsExpandable = true;

        public List<GlGuiElement> Childs = new List<GlGuiElement>();
        public override void Draw(BaseGlDrawingContext dc)
        {
            if (!Visible) return;
            DrawingContext = dc;
            var b = Drawer.CurrentBound;
            Rect.Parent = b;
            Rect.Update();


            var y = dc.Height - Rect.Top;

            var d = dc.Height - Rect.Bottom;

            var g2 = new GuiBounds(Rect.XOffset, Rect.YOffset, Rect.Width, titleH, Rect.Anchor);
            g2.Parent = Rect.Parent;
            g2.Update();



            if (IsExpanded)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                /*GL.Color3(Color.FromArgb(10, 73, 83, 89));
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex3(x, y, 0);
                GL.Vertex3(r, y, 0);
                GL.Vertex3(r, d, 0);
                GL.Vertex3(x, d, 0);
                GL.End();*/

                Drawer.SetColor(73, 83, 89, 164);
                Drawer.FillRectangle(Rect);
                GL.Disable(EnableCap.Blend);
            }
            if (IsHovered)
            {
                Drawer.SetColor(73, 83, 89);
                //GL.Color3(Color.FromArgb(73, 83, 89));
            }
            else
            {
                Drawer.SetColor(53, 63, 69);
                //GL.Color3(Color.FromArgb(53, 63, 69));
            }


            Drawer.FillRectangle(g2);
            Drawer.SetColor(125, 125, 180);
            if (IsExpandable)
            {
                if (IsExpanded)
                {
                    Drawer.FillTriangle(g2.Right - 20, g2.Bottom - titleH / 2 + 5, 10, -10);
                }
                else
                {
                    Drawer.FillTriangle(g2.Right - 20, g2.Top + titleH / 2 - 5, 10, 10);
                }
            }
            /*GL.Begin(PrimitiveType.Quads);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(r, y, 0);
            GL.Vertex3(r, y1, 0);
            GL.Vertex3(x, y1, 0);
            GL.End();*/

            GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);
            if (IsExpanded)
            {
                Drawer.DrawRectangle(Rect.Left, Rect.Top, Rect.Width, Rect.Height);
            }

            Drawer.DrawRectangle(g2);
            /*
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(Rect.Left, y, 0);
            GL.Vertex3(R, y, 0);
            GL.Vertex3(r, y1, 0);
            GL.Vertex3(x, y1, 0);
            GL.End();*/
            GL.LineWidth(1);
            GL.PushMatrix();
            float f1 = 0.3f;

            Drawer.Translate(Rect.Left, Rect.Top);
            GL.Scale(f1, f1, f1);
            dc.TextRoutine.Shader.PushState();
            dc.TextRoutine.Shader.Color = new Vector3(1, 1, 1);
            dc.TextRoutine.DrawText(Title, new PointF(0, 0));
            dc.TextRoutine.Shader.PopState();

            GL.PopMatrix();

            if (IsExpanded)
            {
                var temp = Drawer.CurrentBound;
                Drawer.CurrentBound = Rect;
                foreach (var item in Childs)
                {
                    item.Parent = this;
                    item.Draw(dc);
                }
                Drawer.CurrentBound = temp;
            }
        }

        public override void Update(BaseGlDrawingContext dc)
        {

            foreach (var item in Childs)
            {
                item.Update(dc);
            }
        }
    }
}

