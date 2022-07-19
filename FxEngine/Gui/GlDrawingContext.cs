using OpenTK;
using System.Drawing;

namespace FxEngine.Gui
{

    public class GlDrawingContext : BaseGlDrawingContext
    {
        public override bool Focused { get => GameWindow.Focused; }
        public override string Title { get => GameWindow.Title; }

        public GameWindow GameWindow;
        public override int Width { get => GameWindow.Width; set => GameWindow.Width = value; }
        public override int Height { get => GameWindow.Height; set => GameWindow.Height = value; }
        public override void MakeCurrent()
        {
            GameWindow.MakeCurrent();
            
        }
        public override Point PointToClient(Point cursorPosition)
        {
            return GameWindow.PointToClient(cursorPosition);
        }

        public override void Exit()
        {
            GameWindow.Exit();
        }
    }

}
