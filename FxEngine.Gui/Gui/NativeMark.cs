using OpenTK.Graphics.OpenGL;
using System.Drawing;
using FxEngine.Tiles;

namespace FxEngine.Gui
{
    public class NativeMark : NativeGlGuiElement
    {
        public Tile Tile;
        public override void Draw(BaseGlDrawingContext dc)
        {
            if (!Visible) return;

            Rect.Parent = Drawer.CurrentBound;
            Rect.Update();

            GL.PushMatrix();           



            Drawer.Translate(Rect.Left, Rect.Top);
            float f1 = 0.5f;
            GL.Scale(f1, f1, f1);
            Tile.BlendFunc1 = true;
            Tile.Draw();

            GL.PopMatrix();
            

            base.Draw(dc);
        }        
    }
}
