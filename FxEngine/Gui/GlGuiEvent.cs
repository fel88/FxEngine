using System.Drawing;

namespace FxEngine.Gui
{
    public class GlGuiEvent
    {
        public bool FocusChanged;
        public GlGuiElement NewFocusElement;
        public Point Position;
        public bool Handled;
    }

}
