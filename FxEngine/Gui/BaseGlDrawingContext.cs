using FxEngine.Cameras;
using FxEngine.Fonts.SDF;
using System.Drawing;

namespace FxEngine.Gui
{
    public abstract class BaseGlDrawingContext : DrawingContext
    {
        public SdfTextRoutine TextRoutine;

        public Camera Camera;
        public GameScreen Screen;

        public abstract bool Focused { get;  }
        public abstract string Title { get;  }

        public abstract void MakeCurrent();

        public abstract Point PointToClient(Point cursorPosition);

        public abstract void Exit();
    }

}
