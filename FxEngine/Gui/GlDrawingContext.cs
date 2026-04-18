using OpenTK;
using OpenTK.Windowing.Desktop;
using System.Drawing;

namespace FxEngine.Gui
{

    public class GlDrawingContext : BaseGlDrawingContext
    {
        public override bool Focused { get => GameWindow.IsFocused; }
        public override string Title { get => GameWindow.Title; }

        public OpenTK.Windowing.Desktop.GameWindow GameWindow { get => GameControl as GameWindow; }
        public override int Width
        {
            get
            {
                return GameControl.Width();
            }
            set => GameWindow.Size = new OpenTK.Mathematics.Vector2i(value, GameControl.Height());
        }
        public override int Height { get => GameControl.Height(); set => GameWindow.Size = new OpenTK.Mathematics.Vector2i(GameControl.Width(), value); }
        public override void MakeCurrent()
        {
            GameWindow.MakeCurrent();

        }
        public override Point PointToClient(Point cursorPosition)
        {
            return GameWindow.PointToClient(cursorPosition.ToVector2i()).ToPoint();
        }

        public override void Exit()
        {
            GameWindow.Close();
            //GameWindow.Exit();
        }
    }

}
