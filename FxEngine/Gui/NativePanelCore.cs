using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace FxEngine.Gui
{
    public class NativePanelCore : NativeGlGuiElement
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


            Drawer.RectShader.Use();
            var proj = dc.Camera.ProjectionMatrix;
            var view = dc.Camera.ViewMatrix;
            var model = Matrix4.CreateScale(Rect.Width, Rect.Height, 1);

            var ww1 = dc.Camera.viewport[2];
            var hh1 = dc.Camera.viewport[3];
            model *= Matrix4.CreateTranslation(-ww1 / 2, hh1 / 2, 1);

            model *= Matrix4.CreateTranslation(Rect.Left, -Rect.Top, 1);

            Drawer.RectShader.SetTransform(model * view * proj);
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

              


                Drawer.RectShader.SetColor(new Vector4(73 / 255f, 83 / 255f, 89 / 255f, 164 / 255f));
                Drawer.FillRectangleCore(Rect);
                GL.Disable(EnableCap.Blend);
            }
            if (IsHovered)
            {
                //Drawer.SetColor(73, 83, 89);
                Drawer.RectShader.SetVec4("color", new Vector4(73 / 255f, 83 / 255f, 89 / 255f, 255 ));
                //GL.Color3(Color.FromArgb(73, 83, 89));
            }
            else
            {
                Drawer.RectShader.SetColor( new Vector4(53 / 255f, 63 / 255f, 69 / 255f, 255));

                //Drawer.SetColor(53, 63, 69);
                //GL.Color3(Color.FromArgb(53, 63, 69));
            }


            Drawer.FillRectangleCore(g2);
            Drawer.RectShader.SetColor(new Vector4(125 / 255f, 125 / 255f, 180 / 255f, 255 / 255f));

            //Drawer.SetColor(125, 125, 180);
            if (IsExpandable)
            {
                if (IsExpanded)
                {
                    Drawer.FillTriangleCore(g2.Right - 20, g2.Bottom - titleH / 2 + 5, 10, -10);
                }
                else
                {
                    Drawer.FillTriangleCore(g2.Right - 20, g2.Top + titleH / 2 - 5, 10, 10);
                }
            }
            /*GL.Begin(PrimitiveType.Quads);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(r, y, 0);
            GL.Vertex3(r, y1, 0);
            GL.Vertex3(x, y1, 0);
            GL.End();*/
            Drawer.RectShader.SetColor( new Vector4(118 / 255f, 141 / 255f, 155 / 255f, 255 / 255f));

            //GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);
            if (IsExpanded)
            {
                Drawer.DrawRectangleCore(Rect.Left, Rect.Top, Rect.Width, Rect.Height);
            }

            Drawer.DrawRectangleCore(g2);
            /*
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(Rect.Left, y, 0);
            GL.Vertex3(R, y, 0);
            GL.Vertex3(r, y1, 0);
            GL.Vertex3(x, y1, 0);
            GL.End();*/
            GL.LineWidth(1);
            /*GL.PushMatrix();
            float f1 = 0.3f;
            */
            Drawer.TranslateAccum(Rect.Left, Rect.Top);
            /*
            GL.Scale(f1, f1, f1);
            dc.TextRoutine.Shader.PushState();
            dc.TextRoutine.Shader.Color = new Vector3(1, 1, 1);
            dc.TextRoutine.DrawText(Title, new PointF(0, 0));
            dc.TextRoutine.Shader.PopState();

            GL.PopMatrix();*/

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

