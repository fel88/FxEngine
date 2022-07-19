using OpenTK;
using System;
using System.Drawing;

namespace FxEngine.Gui
{
    public class GlControlDrawingContext : BaseGlDrawingContext
    {
        public string _title { get; set; }
        public override string Title { get => _title; }

        public override bool Focused { get => GameWindow.Focused; }

        public GLControl GameWindow;
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
            throw new NotImplementedException();
        }
    }

}
