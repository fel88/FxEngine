using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using FxEngine.Fonts.SDF;

namespace FxEngine.Gui
{
    public class NativeButtonCore : NativeGlGuiElement
    {
        public GameSound ClickSound;
        public override void Event(BaseGlDrawingContext dc, GlGuiEvent ev)
        {
            if (!Enabled) return;
            if (Parent is NativePanel)
            {
                if (!(Parent as NativePanel).IsChildsVisible)
                {
                    return;
                }
            }
            if (!Visible) return;
            IsHovered = false;
            if (Rect.IntersectsWith(ev.Position.X, ev.Position.Y))
            {
                IsHovered = true;
                if (Hovered != null)
                {
                    Hovered();
                }
            }

            if (ev is MouseClickGlGuiEvent)
            {
                if (IsHovered)
                {
                    if (Click != null)
                    {
                        if (ClickSound != null)
                        {
                            AudioStuff.Stuff.Play(ClickSound.Path);
                        }
                        Click();
                    }
                    ev.Handled = true;
                }
            }
            base.Event(dc, ev);
        }
        public bool IsHovered = false;
        public string Caption;

        BaseGlDrawingContext DrawingContext;
        public Action Click { get; set; }
        public Action Hovered { get; set; }

        public override void Draw(BaseGlDrawingContext dc)
        {
            if (!Visible) return;
            DrawingContext = dc;            

            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();

            Drawer.RectShader.Use();
            if (!Enabled)
            {
                //GL.Color3(Color.LightGray);
                Drawer.RectShader.SetColor(Color.LightGray);
            }
            else
            {
                if (IsHovered)
                {
                    //GL.Color3(Color.FromArgb(73, 83, 89));
                    Drawer.RectShader.SetColor(Color.FromArgb(73, 83, 89));
                }
                else
                {
                    //GL.Color3(Color.FromArgb(53, 63, 69));
                    Drawer.RectShader.SetColor(Color.FromArgb(53, 63, 69));
                }
            }

            var proj = dc.Camera.ProjectionMatrix;
            var view = dc.Camera.ViewMatrix;
            var model = Matrix4.CreateScale(Rect.Width, Rect.Height, 1);

            var ww1 = dc.Camera.viewport[2];
            var hh1 = dc.Camera.viewport[3];
            model *= Matrix4.CreateTranslation(-ww1 / 2, hh1 / 2, 1);

            model *= Matrix4.CreateTranslation(Rect.Left, -Rect.Top, 1);

            Drawer.RectShader.SetTransform(model * view * proj);


            Drawer.FillRectangleCore(Rect);


            Drawer.RectShader.SetColor(Color.FromArgb(118, 141, 155));

            GL.LineWidth(1.3f);
            var er = GL.GetError();
            Drawer.DrawRectangleCore(Rect);

            GL.LineWidth(1);
            //   GL.PushMatrix();

            DrawText(dc);
          
        }


        public void DrawText(BaseGlDrawingContext ctx)
        {
            var isdep = GL.GetBoolean(GetPName.DepthTest);
            var isblend = GL.GetBoolean(GetPName.Blend);
            GL.Disable(EnableCap.DepthTest);

            var rgs = ctx.TextRoutine.kr.GetStringRegions(Caption);
            var width = rgs.Last().Bound.Right * TextScale;
                        
            var cb = Drawer.CurrentBound;
            var ww1 = cb.Width;
            var hh1 = cb.Height;
                        
            ctx.TextRoutine.shader.Use();
            var sdf = (ctx.TextRoutine.shader as SdfShader);

            var proj = ctx.Camera.ProjectionMatrix;
            sdf.SetMatrix4("projection", proj);
            var view2 = Matrix4.LookAt(new Vector3(0, 0, 1f), Vector3.Zero, Vector3.UnitY);
            sdf.Color = new Vector3(1, 1, 1);

            sdf.SetTexture();

            //ctx.TextRoutine.shader.SetUniformsData();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            for (int i = 0; i < Caption.Length; i++)
            {
                var s = Caption[i];
                var fr = ctx.TextRoutine.infos.First(z => z.Key == s).Value;
                
                Matrix4 local3 = Matrix4.Identity;
                var dp = rgs[i].DrawPoint;

                var offset = fr.Char.Offset;
                local3 *= Matrix4.CreateTranslation(offset.X, -offset.Y, 0);

                local3 *= Matrix4.CreateTranslation(dp.X, dp.Y, 0);               

                local3 *= Matrix4.CreateScale(TextScale);

                local3 *= Matrix4.CreateTranslation(-ww1 / 2, hh1 / 2, 0);
                local3 *= Matrix4.CreateTranslation(Rect.Left, -Rect.Top, 0);
                local3 *= Matrix4.CreateTranslation(Drawer.ShiftX, Drawer.ShiftY, 0);


                if (TextAlign == GlGuiTextAlign.Center)
                {
                    local3 *= Matrix4.CreateTranslation(Rect.Width / 2 - width / 2, 0, 0);
                }

                sdf.SetMatrix4("transformation", local3 * view2);
                GL.BindVertexArray(fr.VertexBuffer);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }

        }
        public float TextScale = 0.3f;

        public override void Update(BaseGlDrawingContext dc)
        {

        }
    }
}
