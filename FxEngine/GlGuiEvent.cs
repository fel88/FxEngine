using FxEngine.Interfaces;
using System.Drawing;

namespace FxEngine.Gui
{
    public class GlGuiEvent
    {
        public bool FocusChanged;
        public IGuiElement NewFocusElement;
        public Point Position;
        public bool Handled;
    }

}
