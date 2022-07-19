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
    public class NativeLabel : NativeGlGuiElement
    {

        public string Text;
        public Action<NativeLabel> Updater;

        BaseGlDrawingContext DrawingContext;

        public float FontScale = 0.3f;

        public float SdfGamma = 0.15f;
        public override void Draw(BaseGlDrawingContext dc)
        {
            DrawingContext = dc;
            var bound = Drawer.CurrentBound;
            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();

            
            GL.PushMatrix();

            GL.Color3(Color.FromArgb(53, 63, 69));

            float f1 = FontScale;
            Drawer.Translate(Rect.Left, Rect.Top);
            //GL.Translate(x, dc.GameWindow.Height - (Rect.Top), 0);
            GL.Scale(f1, f1, f1);

            dc.TextRoutine.Shader.PushState();
            dc.TextRoutine.SetGamma(SdfGamma);
            dc.TextRoutine.Shader.Color = new Vector3(1, 1, 1);
            dc.TextRoutine.DrawText(Text, new PointF(0, 0));
            
            dc.TextRoutine.Shader.PopState();
            GL.PopMatrix();

        }

        public override void Update(BaseGlDrawingContext dc)
        {
            if (Updater != null)
            {
                Updater(this);
            }
        }
    }
}
