using System.Linq;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;

namespace FxEngine.Gui
{
    public class NativeTextBox : NativeGlGuiElement
    {
        BaseGlDrawingContext DrawingContext;

        public Action<NativeTextBox> EnterAction;
        public Action<NativeTextBox> TextChanged;
        public int Position;
        public override void Event(BaseGlDrawingContext dc, GlGuiEvent ev)
        {
            if (ev is MouseClickGlGuiEvent)
            {
                if (Rect.IntersectsWith(ev.Position.X, ev.Position.Y))
                {
                    var rgs = dc.TextRoutine.kr.GetStringRegions(Text);
                    var xx = ev.Position.X - Rect.Left;
                    for (int i = 0; i < rgs.Length; i++)
                    {
                        var sx1 = rgs[i].Bound.Left * TextScaler;
                        var sx2 = rgs[i].Bound.Right * TextScaler;
                        if (xx >= sx1 && xx <= sx2)
                        {
                            Position = i;
                        }
                    }
                    ev.FocusChanged = true;
                    ev.NewFocusElement = this;
                    ev.Handled = true;
                    Focused = true;
                }
            }
            if (ev is KeyGlGuiEvent && Focused)
            {
                var kev = ev as KeyGlGuiEvent;
                if (kev.Key == Key.Enter)
                {
                    if (EnterAction != null)
                    {
                        EnterAction(this);

                    }
                }
                else
                if (kev.Key == Key.Comma || kev.Key == Key.Period)
                {
                    Text = Text.Insert(Position, ".");
                    Position++;
                }
                else
                if (kev.Key >= Key.Number0 && kev.Key <= Key.Number9)
                {
                    var ind = (kev.Key - Key.Number0);
                    Text = Text.Insert(Position, (char)('0' + ind) + "");
                    Position++;
                }
                else
                     if (kev.Key >= Key.Keypad0 && kev.Key <= Key.Keypad9)
                {
                    var ind = (kev.Key - Key.Keypad0);
                    Text = Text.Insert(Position, (char)('0' + ind) + "");
                    Position++;
                }
                else
                     if (kev.Key == Key.Left)
                {
                    Position--;
                    if (Position < 0) { Position = 0; }
                }
                else
                     if (kev.Key == Key.Right)
                {
                    Position++;
                    if (Position > Text.Length)
                    {
                        Position = Text.Length;
                    }
                }
                else
                if (kev.Key == Key.BackSpace)
                {
                    if (Text.Any() && Position > 0)
                    {
                        Text = Text.Remove(Position - 1, 1);
                        Position--;
                    }
                }
                else
                if (kev.Key == Key.Delete)
                {
                    if (Text.Any() && Position < Text.Length)
                    {
                        Text = Text.Remove(Position, 1);
                    }
                }
                else
                {
                    Text += kev.Key;
                }
                if (TextChanged != null)
                {
                    TextChanged(this);
                }

            }
            base.Event(dc, ev);
        }
        float TextScaler = 0.3f;
        public string Text;
        public override void Draw(BaseGlDrawingContext dc)
        {
            DrawingContext = dc;
            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();

            var x = Rect.Left;

            var top = Rect.Top;

            var y = top;
            var r = Rect.Right;
            var d = Rect.Bottom;
            GL.PushMatrix();

            if (Focused)
            {
                GL.Color3(Color.LightSalmon);
                GL.Begin(PrimitiveType.Quads);
                Drawer.Vertex(x, d);
                Drawer.Vertex(x + Rect.Width, d);
                Drawer.Vertex(x + Rect.Width, y);
                Drawer.Vertex(x, y);
                GL.End();

            }
            else
            {
                GL.Color3(Color.Beige);
                GL.Begin(PrimitiveType.LineLoop);
                Drawer.Vertex(x, d);
                Drawer.Vertex(x + Rect.Width, d);
                Drawer.Vertex(x + Rect.Width, y);
                Drawer.Vertex(x, y);
                GL.End();
            }

            GL.Color3(Color.FromArgb(118, 141, 155));
            GL.LineWidth(1.3f);

            Drawer.DrawRectangle(Rect);

            GL.PopMatrix();
            GL.PushMatrix();

            GL.Color3(Color.FromArgb(53, 63, 69));


            Drawer.Translate(Rect.Left, Rect.Top);
            var f1 = TextScaler;
            GL.Scale(f1, f1, f1);

            dc.TextRoutine.Shader.PushState();
            dc.TextRoutine.Shader.Color = new Vector3(1, 1, 1);

            var rgs = dc.TextRoutine.kr.GetStringRegions(Text);
            dc.TextRoutine.DrawText(Text, new PointF(0, 0));
            dc.TextRoutine.Shader.PopState();
            GL.PopMatrix();

            //draw cursor

            if (!BlinkSw.IsRunning)
            {
                BlinkSw.Start();
            }
            if (BlinkSw.ElapsedMilliseconds > 500)
            {

                BlinkSw.Restart();
                CursorVisible = !CursorVisible;
            }

            if (CursorVisible && Focused)
            {
                float sum = 0;
                if (Position > 0)
                {
                    sum = rgs[Position - 1].Bound.Right * f1;
                }

                GL.Color3(Color.White);
                GL.Begin(PrimitiveType.Lines);
                Drawer.Vertex(x + sum, top + 2);
                Drawer.Vertex(x + sum, d - 2);
                GL.End();
            }


            base.Draw(dc);
        }
        public Stopwatch BlinkSw = new Stopwatch();
        public bool CursorVisible = true;
    }
}
